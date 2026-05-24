using CleanCity.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Notifications
{
    public class CreateNotificationDto
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string? DeepLink { get; set; }

        public int? ReportId { get; set; }
        public int? ReportAssignmentId { get; set; }
        public int? AwarenessContentId { get; set; }
        public int? AIAnalysisResultId { get; set; }
    }
}
