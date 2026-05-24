using CleanCity.Domain.Enum;

namespace CleanCity.Application.DTOs.Reports
{
    /// <summary>
    /// معاملات الفلترة والبحث لصفحة قائمة البلاغات.
    /// </summary>
    public class ReportFilterDto
    {
        /// <summary>تصفية حسب الحالة (اختياري).</summary>
        public ReportStatus? Status { get; set; }

        /// <summary>تصفية حسب المنطقة (اختياري).</summary>
        public int? AreaId { get; set; }

        /// <summary>من تاريخ (اختياري).</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>إلى تاريخ (اختياري).</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>بحث نصي: رقم البلاغ، وصف، اسم سائق (اختياري).</summary>
        public string? Search { get; set; }

        /// <summary>رقم الصفحة الحالية (يبدأ من 1).</summary>
        public int Page { get; set; } = 1;

        /// <summary>عدد السجلات في كل صفحة.</summary>
        public int PageSize { get; set; } = 20;
    }
}