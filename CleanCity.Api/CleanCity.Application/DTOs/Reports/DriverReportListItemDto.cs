using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class DriverReportListItemDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CurrentDriverId { get; set; }
        public string ReportStatus { get; set; } = string.Empty;
        public string AssignmentStatus { get; set; } = string.Empty;
        public string UiBucket { get; set; } = string.Empty;
    }
}
