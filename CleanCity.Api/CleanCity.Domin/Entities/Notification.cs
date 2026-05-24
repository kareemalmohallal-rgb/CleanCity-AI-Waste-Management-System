using CleanCity.Domain.Enum;
using CleanCity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;

        // رابط داخلي للتطبيق (اختياري)
        public string? DeepLink { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ===== Source Links (يرتبط بكل الكلاسات الممكنة) =====

        // 1) مرتبط ببلاغ
        public int? ReportId { get; set; }
        public Report? Report { get; set; }

        // 2) مرتبط بإسناد بلاغ لسائق
        public int? ReportAssignmentId { get; set; }
        public ReportAssignment? ReportAssignment { get; set; }

        // 3) مرتبط بمحتوى توعوي
        public int? AwarenessContentId { get; set; }
        public AwarenessContent? AwarenessContent { get; set; }

        // 4) مرتبط بنتيجة تحليل AI
        public int? AIAnalysisResultId { get; set; }
        public AIAnalysisResult? AIAnalysisResult { get; set; }

        // ===== Recipients =====
        public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
    }
}
