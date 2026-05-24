using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanCity.Domain.Enum;

namespace CleanCity.Domain.Entities
{
    public class Report
    {
        public int Id { get; set; }

        public string? ImageUrl { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string? Description { get; set; }
        // ✅ السائق الحالي للبلاغ (آخر من تم إسناده فعليًا)
        public int? CurrentDriverId { get; set; }
        public Driver? CurrentDriver { get; set; }
        public ReportStatus Status { get; set; }
        public int AnonymousDeviceId { get; set; }
        public AnonymousDevice? AnonymousDevice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public AIAnalysisResult? AIAnalysisResult { get; set; }
        public ICollection<FileEntity>? Files { get; set; } = new List<FileEntity>();
        public ICollection<ReportAssignment>? Assignments { get; set; } = new List<ReportAssignment>();
        public ICollection<ReportStatusHistory>? StatusHistories { get; set; } = new List<ReportStatusHistory>();



    }

}
