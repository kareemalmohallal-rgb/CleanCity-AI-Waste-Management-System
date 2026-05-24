using CleanCity.Domain.Enum;

namespace CleanCity.Application.DTOs.Notifications
{
    /// <summary>
    /// DTO يجمع بيانات المستلم والإشعار معاً بدون Navigation Properties
    /// لتجنب الـ circular reference عند الـ JSON serialization
    /// </summary>
    public class NotificationRecipientWithDetailsDto
    {
        // ─── بيانات المستلم ───────────────────────────────────────────────────
        public int Id { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? AnonymousDeviceId { get; set; }
        public int? DriverId { get; set; }
        public string? UserId { get; set; }
        public int? DeviceTokenId { get; set; }

        // ─── بيانات الإشعار ───────────────────────────────────────────────────
        public NotificationReadDto Notification { get; set; } = default!;
    }
}