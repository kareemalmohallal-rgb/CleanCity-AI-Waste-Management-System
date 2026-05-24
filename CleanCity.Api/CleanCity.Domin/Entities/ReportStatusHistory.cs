using CleanCity.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class ReportStatusHistory
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public ReportStatus OldStatus { get; set; }
        public ReportStatus NewStatus { get; set; }
        public string? UpdatedByUserId { get; set; }
        public string UpdatedByType { get; set; } // System / Admin / Driver
        public DateTime UpdatedAt { get; set; }
        public Report Report { get; set; }
    }
}
