namespace CleanCity.MVC.Models;

public sealed class HotspotAreaVm
{
    public int AreaId { get; set; }
    public string AreaName { get; set; } = "";
    public int ReportsCount { get; set; }
}