using CleanCity.Application.Common;
using CleanCity.Application.DTOs.Reports;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IReportRepository
    {
        // ─── الموجودة سابقاً (لا تغيير) ───────────────────────────────────────
        Task<Report?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Report?> GetByIdWithAssignmentsAsync(int id, CancellationToken ct = default);
        Task AddAsync(Report report, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<List<DriverReportListItemDto>> GetReportsByDriverIdAsync(int driverId, CancellationToken ct);

        // ─── جديد: صفحة التقارير بلوحة الإدارة ───────────────────────────────
        /// <summary>
        /// يُرجع قائمة البلاغات مُقسَّمة إلى صفحات مع دعم الفلترة والبحث.
        /// </summary>
        Task<PagedResult<ReportAdminListItemDto>> GetPagedAsync(
            ReportFilterDto filter,
            CancellationToken ct = default);
    }
}