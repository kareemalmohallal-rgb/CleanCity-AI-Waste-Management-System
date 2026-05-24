using CleanCity.Application.Common;
using CleanCity.Application.DTOs.Reports;
using CleanCity.Domain.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanCity.MVC.Models
{
    /// <summary>
    /// ViewModel كاملة لصفحة قائمة البلاغات بلوحة الإدارة.
    /// تحتوي على: نتائج الصفحة الحالية، قيم الفلاتر، وقوائم الاختيار.
    /// </summary>
    public sealed class ReportListVm
    {
        // ── النتائج المُرشَّحة والمُقسَّمة ────────────────────────────────────
        public PagedResult<ReportAdminListItemDto> Paged { get; set; } = new();

        // ── قيم الفلاتر الحالية (لإبقاء الفلاتر محددةً بعد التقديم) ──────────
        public ReportStatus? FilterStatus { get; set; }
        public int? FilterAreaId { get; set; }
        public DateTime? FilterDateFrom { get; set; }
        public DateTime? FilterDateTo { get; set; }
        public string? FilterSearch { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // ── قوائم الاختيار لعناصر الفلتر ─────────────────────────────────────
        public List<SelectListItem> AreaOptions { get; set; } = new();

        // خيارات الحالة مضمَّنة مباشرةً في الـ View لتجنب بناء الـ SelectListItem في الـ ViewModel

        // ── مساعدات العرض ──────────────────────────────────────────────────
        /// <summary>
        /// تُرجع CSS class مناسب لشارة الحالة.
        /// </summary>
        public static string StatusBadgeClass(string statusKey) => statusKey switch
        {
            "completed" => "badge badge--success",
            "in_progress" => "badge badge--info",
            "under_review" => "badge badge--warn",
            "rejected" => "badge badge--neutral",
            _ => "badge badge--neutral"   // received
        };
    }
}