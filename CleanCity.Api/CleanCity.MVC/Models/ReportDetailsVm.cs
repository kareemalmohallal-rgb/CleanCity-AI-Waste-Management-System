using System;

namespace CleanCity.MVC.Models;

public sealed class ReportDetailsVm
{
    public int ReportId { get; set; }
    public string AreaName { get; set; } = "—";
    public string Description { get; set; } = "—";
    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = "—";
    public string? CurrentDriverName { get; set; }
    public string? AiReason { get; set; }
    // آخر رفض
    public string? RejectedByDriverName { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? ImageUrl { get; set; }
}