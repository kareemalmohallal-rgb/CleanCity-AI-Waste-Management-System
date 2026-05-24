using CleanCity.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class UpdateReportStatusDto
    {
        public ReportStatus NewStatus { get; set; }
        public string UpdatedByType { get; set; } = "System"; 
        public string? UpdatedByUserId { get; set; }
    }
}
