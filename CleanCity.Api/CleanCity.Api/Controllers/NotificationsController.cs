using CleanCity.Application.DTOs.Notifications;
using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notifications;
        private readonly IDriverRepository _drivers;
        private readonly IUnitOfWork _uow;

        public NotificationsController(
            INotificationService notifications,
            IDriverRepository drivers,
            IUnitOfWork uow)
        {
            _notifications = notifications;
            _drivers = drivers;
            _uow = uow;
        }

        [HttpGet("driver/{driverId:int}")]
        public async Task<ActionResult<List<NotificationRecipientWithDetailsDto>>> GetForDriver(
            int driverId,
            CancellationToken ct)
        {
            var items = await _notifications.GetForDriverAsync(driverId, ct);
            return Ok(items.Select(ToDto).ToList());
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<NotificationRecipientWithDetailsDto>>> GetForUser(
            string userId,
            CancellationToken ct)
        {
            var items = await _notifications.GetForUserAsync(userId, ct);
            return Ok(items.Select(ToDto).ToList());
        }

        [HttpGet("device/{anonymousDeviceId:int}")]
        public async Task<ActionResult<List<NotificationRecipientWithDetailsDto>>> GetForAnonymousDevice(
            int anonymousDeviceId,
            CancellationToken ct)
        {
            var items = await _notifications.GetForAnonymousDeviceAsync(anonymousDeviceId, ct);
            return Ok(items.Select(ToDto).ToList());
        }

        [HttpPost("{recipientId:int}/read")]
        public async Task<ActionResult> MarkAsRead(int recipientId, CancellationToken ct)
        {
            await _notifications.MarkAsReadAsync(recipientId, ct);
            return NoContent();
        }

        [HttpPost("register-driver-token")]
        public async Task<ActionResult> RegisterDriverToken(
            [FromBody] RegisterDriverTokenDto dto,
            CancellationToken ct)
        {
            if (dto.DriverId <= 0)
                return BadRequest("DriverId is required.");
            if (string.IsNullOrWhiteSpace(dto.FcmToken))
                return BadRequest("FcmToken is required.");

            var driver = await _drivers.GetByIdAsync(dto.DriverId, ct);
            if (driver == null)
                return NotFound("Driver not found.");

            driver.FcmToken = dto.FcmToken.Trim();
            await _uow.SaveChangesAsync(ct);
            return NoContent();
        }

        // ─── Helper: تحويل Entity إلى DTO بدون Navigation Properties ──────────
        private static NotificationRecipientWithDetailsDto ToDto(NotificationRecipient r)
        {
            return new NotificationRecipientWithDetailsDto
            {
                Id = r.Id,
                IsRead = r.IsRead,
                ReadAt = r.ReadAt,
                AnonymousDeviceId = r.AnonymousDeviceId,
                DriverId = r.DriverId,
                UserId = r.UserId,
                DeviceTokenId = r.DeviceTokenId,
                Notification = new NotificationReadDto
                {
                    Id = r.Notification.Id,
                    Type = r.Notification.Type,
                    Title = r.Notification.Title,
                    Body = r.Notification.Body,
                    DeepLink = r.Notification.DeepLink,
                    CreatedAt = r.Notification.CreatedAt,
                    ReportId = r.Notification.ReportId,
                    ReportAssignmentId = r.Notification.ReportAssignmentId,
                    AwarenessContentId = r.Notification.AwarenessContentId,
                    AIAnalysisResultId = r.Notification.AIAnalysisResultId,
                },
            };
        }
    }
}