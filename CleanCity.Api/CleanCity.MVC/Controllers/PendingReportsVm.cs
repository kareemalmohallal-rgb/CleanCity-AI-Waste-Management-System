using System;

namespace CleanCity.MVC.Models;

public sealed class PendingReportsVm
{
    public List<DriverRejectedReportRow> DriverRejected { get; set; } = new();
    public List<AiRejectedReportRow> AiRejected { get; set; } = new();
}

public sealed class DriverRejectedReportRow
{
    public int ReportId { get; set; }
    public string AreaName { get; set; } = "—";
    public string DriverName { get; set; } = "—";
    public string? RejectionReason { get; set; }
    public DateTime? RejectedAt { get; set; }
}

public sealed class AiRejectedReportRow
{
    public int ReportId { get; set; }
    public string AreaName { get; set; } = "—";
    public string? AiReason { get; set; }
    public decimal? Confidence { get; set; }
    public string SuggestedDriverName { get; set; } = "—";
    public DateTime? DecidedAt { get; set; }
}