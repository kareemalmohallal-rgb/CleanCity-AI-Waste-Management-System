using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanCity.MVC.Models;

public sealed class ReassignReportVm
{
    public int ReportId { get; set; }
    public int AreaId { get; set; }
    public string AreaName { get; set; } = "—";

    public int? SelectedDriverId { get; set; }
    public List<SelectListItem> Drivers { get; set; } = new();
}