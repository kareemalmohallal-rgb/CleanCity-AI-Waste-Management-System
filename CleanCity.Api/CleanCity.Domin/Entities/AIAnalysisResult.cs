using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class AIAnalysisResult
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public Report? Report { get; set; }
        public bool IsGarbageDetected { get; set; }
        public decimal ConfidencePercentage { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
