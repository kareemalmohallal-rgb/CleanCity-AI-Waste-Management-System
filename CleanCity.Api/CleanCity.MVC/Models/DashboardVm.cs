namespace CleanCity.MVC.Models;

public sealed class DashboardVm
{
    // Stats
    public int ReportsToday { get; set; }
    public int PendingReports { get; set; }
    public int CompletedLast24h { get; set; }
    public double AvgResponseMinutes { get; set; } // من الاستلام حتى الإسناد

    // Filters dropdown
    public List<AreaOption> Areas { get; set; } = new();

    // Latest reports
    public List<LatestReportVm> LatestReports { get; set; } = new();

    // Hotspots
    public List<HotspotVm> Hotspots { get; set; } = new();
}

public sealed class AreaOption
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public sealed class LatestReportVm
{
    public int Id { get; set; }
    public string AreaName { get; set; } = "—";
    public string StatusText { get; set; } = "—";
    public string StatusBadgeClass { get; set; } = "badge badge--neutral";
    public string DriverName { get; set; } = "—";
    public DateTime CreatedAt { get; set; }
}

public sealed class HotspotVm
{
    public string AreaName { get; set; } = "";
    public int Count { get; set; }
    public int Percent { get; set; } // 0..100
}