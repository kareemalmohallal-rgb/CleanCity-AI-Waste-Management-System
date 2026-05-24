using CleanCity.Application.Common;
using CleanCity.Application.DTOs.Reports;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IReportService
    {
        // ─── الموجودة سابقاً (لا تغيير) ───────────────────────────────────────
        Task<int> CreateAsync(CreateReportDto dto, CancellationToken ct = default);
        Task AssignToDriverAsync(int reportId, int driverId, string updatedByType, string? updatedByUserId, CancellationToken ct = default);
        Task UpdateStatusAsync(int reportId, UpdateReportStatusDto dto, CancellationToken ct = default);
        Task<Report?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<DriverReportListItemDto>> GetReportsByDriverIdAsync(int driverId, CancellationToken ct);
        Task DriverAcceptAsync(int reportId, int driverId, CancellationToken ct = default);
        Task DriverRejectAsync(int reportId, int driverId, string? rejectionReason, CancellationToken ct = default);
        Task DriverCompleteExecutionAsync(int reportId, int driverId, ReportStatus finalStatus, CancellationToken ct = default);

        // ─── جديد: صفحة التقارير بلوحة الإدارة ───────────────────────────────
        /// <summary>
        /// يُرجع قائمة البلاغات مُقسَّمة إلى صفحات مع دعم الفلترة والبحث.
        /// </summary>
        Task<PagedResult<ReportAdminListItemDto>> GetPagedAsync(
            ReportFilterDto filter,
            CancellationToken ct = default);
    }
}