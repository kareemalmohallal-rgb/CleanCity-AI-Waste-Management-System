using CleanCity.Application.DTOs.Reports;
using CleanCity.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reports;

        public ReportsController(IReportService reports)
        {
            _reports = reports;
        }

        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            return Ok(new
            {
                AuthHeader = Request.Headers.Authorization.ToString(),
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        [Authorize(Roles = "Driver")]
        [HttpGet("my-assigned")]
        public async Task<ActionResult<List<DriverReportListItemDto>>> GetMyAssignedReports(CancellationToken ct)
        {
            var driverIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == "driverId")?.Value;

            if (string.IsNullOrWhiteSpace(driverIdClaim) || !int.TryParse(driverIdClaim, out var driverId))
                return Unauthorized("DriverId claim not found.");

            var items = await _reports.GetReportsByDriverIdAsync(driverId, ct);
            return Ok(items);
        }

        public sealed class CreateReportForm
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? Description { get; set; }
            public int AnonymousDeviceId { get; set; }
            public IFormFile? Image { get; set; }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateReportForm form, CancellationToken ct)
        {
            Stream? stream = null;
            string? name = null;
            string? type = null;

            if (form.Image is { Length: > 0 })
            {
                stream = form.Image.OpenReadStream();
                name = form.Image.FileName;
                type = form.Image.ContentType;
            }

            var dto = new CreateReportDto
            {
                Latitude = form.Latitude,
                Longitude = form.Longitude,
                Description = form.Description,
                AnonymousDeviceId = form.AnonymousDeviceId,
                ImageStream = stream,
                ImageFileName = name,
                ImageContentType = type
            };

            var id = await _reports.CreateAsync(dto, ct);
            return Ok(new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id, CancellationToken ct)
        {
            var report = await _reports.GetByIdAsync(id, ct);
            if (report == null) return NotFound();
            return Ok(report);
        }

        [HttpPost("{id:int}/assign/{driverId:int}")]
        public async Task<ActionResult> AssignToDriver(
            int id,
            int driverId,
            [FromHeader(Name = "UpdatedByType")] string updatedByType = "Admin",
            [FromHeader(Name = "UpdatedByUserId")] string? updatedByUserId = null,
            CancellationToken ct = default)
        {
            await _reports.AssignToDriverAsync(id, driverId, updatedByType, updatedByUserId, ct);
            return Ok();
        }

        [HttpPut("{id:int}/status")]
        public async Task<ActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateReportStatusDto dto,
            CancellationToken ct = default)
        {
            await _reports.UpdateStatusAsync(id, dto, ct);
            return NoContent();
        }

        [Authorize(Roles = "Driver")]
        [HttpPost("{id:int}/driver-accept")]
        public async Task<ActionResult> DriverAccept(int id, CancellationToken ct)
        {
            var driverIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == "driverId")?.Value;

            if (string.IsNullOrWhiteSpace(driverIdClaim) || !int.TryParse(driverIdClaim, out var driverId))
                return Unauthorized("DriverId claim not found.");

            await _reports.DriverAcceptAsync(id, driverId, ct);
            return NoContent();
        }

        [Authorize(Roles = "Driver")]
        [HttpPost("{id:int}/driver-reject")]
        public async Task<ActionResult> DriverReject(
            int id,
            [FromBody] DriverRejectReportDto dto,
            CancellationToken ct)
        {
            var driverIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == "driverId")?.Value;

            if (string.IsNullOrWhiteSpace(driverIdClaim) || !int.TryParse(driverIdClaim, out var driverId))
                return Unauthorized("DriverId claim not found.");

            await _reports.DriverRejectAsync(id, driverId, dto.RejectionReason, ct);
            return NoContent();
        }
        [Authorize(Roles = "Driver")]
        [HttpPost("{id:int}/driver-complete")]
        public async Task<ActionResult> DriverCompleteExecution(
    int id,
    [FromBody] DriverCompleteReportDto dto,
    CancellationToken ct)
        {
            var driverIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == "driverId")?.Value;

            if (string.IsNullOrWhiteSpace(driverIdClaim) || !int.TryParse(driverIdClaim, out var driverId))
                return Unauthorized("DriverId claim not found.");

            await _reports.DriverCompleteExecutionAsync(id, driverId, dto.FinalStatus, ct);
            return NoContent();
        }
    }
}