using CleanCity.Application.Common;
using CleanCity.Application.DTOs.Reports;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationContext _context;

        public ReportRepository(ApplicationContext context)
        {
            _context = context;
        }

        // ─── الموجودة سابقاً (لا تغيير) ──────────────────────────────────────

        public Task<Report?> GetByIdAsync(int id, CancellationToken ct = default)
            => _context.Reports.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Report?> GetByIdWithAssignmentsAsync(int id, CancellationToken ct = default)
            => _context.Reports
                .Include(r => r.Assignments)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task AddAsync(Report report, CancellationToken ct = default)
            => await _context.Reports.AddAsync(report, ct);

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _context.Reports.AnyAsync(x => x.Id == id, ct);

        public async Task<List<DriverReportListItemDto>> GetReportsByDriverIdAsync(
            int driverId, CancellationToken ct)
        {
            return await _context.Reports
                .AsNoTracking()
                .Where(r => r.CurrentDriverId == driverId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    Report = r,
                    CurrentAssignment = r.Assignments!
                        .Where(a => a.DriverId == driverId)
                        .OrderByDescending(a => a.AssignedAt)
                        .FirstOrDefault()
                })
                .Select(x => new DriverReportListItemDto
                {
                    Id = x.Report.Id,
                    ImageUrl = x.Report.ImageUrl,
                    Latitude = x.Report.Latitude,
                    Longitude = x.Report.Longitude,
                    Description = x.Report.Description,
                    CreatedAt = x.Report.CreatedAt,
                    CurrentDriverId = x.Report.CurrentDriverId,

                    ReportStatus =
                        x.Report.Status == ReportStatus.Received ? "received" :
                        x.Report.Status == ReportStatus.InProgress ? "in_progress" :
                        x.Report.Status == ReportStatus.UnderReview ? "under_review" :
                        x.Report.Status == ReportStatus.Completed ? "completed" :
                        x.Report.Status == ReportStatus.Rejected ? "rejected" : "unknown",

                    AssignmentStatus =
                        x.CurrentAssignment == null ? "unknown" :
                        x.CurrentAssignment.Status == Domain.Enum.AssignmentStatus.Assigned ? "assigned" :
                        x.CurrentAssignment.Status == Domain.Enum.AssignmentStatus.Completed ? "completed" :
                        x.CurrentAssignment.Status == Domain.Enum.AssignmentStatus.Rejected ? "rejected" : "unknown",

                    UiBucket =
                        x.Report.Status == ReportStatus.Completed ? "completed" :
                        x.CurrentAssignment != null &&
                        x.CurrentAssignment.Status == Domain.Enum.AssignmentStatus.Assigned &&
                        x.Report.Status == ReportStatus.Received ? "new" :
                        x.CurrentAssignment != null &&
                        x.CurrentAssignment.Status == Domain.Enum.AssignmentStatus.Completed &&
                        x.Report.Status == ReportStatus.InProgress ? "in_progress" :
                        x.Report.Status == ReportStatus.UnderReview ? "under_review" :
                        x.Report.Status == ReportStatus.Rejected ? "rejected" : "other"
                })
                .ToListAsync(ct);
        }

        // ─── جديد: قائمة البلاغات مُرشَّحة ومُقسَّمة إلى صفحات ───────────────

        public async Task<PagedResult<ReportAdminListItemDto>> GetPagedAsync(
            ReportFilterDto filter,
            CancellationToken ct = default)
        {
            // ── 1. بناء الاستعلام الأساسي مع Include اللازمة ──────────────────
            var query = _context.Reports
                .AsNoTracking()
                .Include(r => r.Area)
                .Include(r => r.CurrentDriver)
                .Include(r => r.AIAnalysisResult)
                .AsQueryable();

            // ── 2. تطبيق الفلاتر ────────────────────────────────────────────
            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status.Value);

            if (filter.AreaId.HasValue)
                query = query.Where(r => r.AreaId == filter.AreaId.Value);

            if (filter.DateFrom.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.DateFrom.Value.Date);

            if (filter.DateTo.HasValue)
                query = query.Where(r => r.CreatedAt < filter.DateTo.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim();

                // بحث برقم البلاغ إذا كان المدخل رقماً
                if (int.TryParse(term, out var reportId))
                {
                    query = query.Where(r => r.Id == reportId);
                }
                else
                {
                    query = query.Where(r =>
                        (r.Description != null && r.Description.Contains(term)) ||
                        (r.CurrentDriver != null && r.CurrentDriver.FullName.Contains(term)) ||
                        (r.Area != null && r.Area.Name.Contains(term)));
                }
            }

            // ── 3. حساب الإجمالي قبل التقسيم ────────────────────────────────
            var totalCount = await query.CountAsync(ct);

            // ── 4. الترتيب والتقسيم إلى صفحات ──────────────────────────────
            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReportAdminListItemDto
                {
                    Id = r.Id,
                    ImageUrl = r.ImageUrl,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Description = r.Description,

                    AreaId = r.AreaId,
                    AreaName = r.Area != null ? r.Area.Name : "—",

                    DriverId = r.CurrentDriverId,
                    DriverName = r.CurrentDriver != null ? r.CurrentDriver.FullName : "—",

                    StatusKey =
                        r.Status == ReportStatus.Received ? "received" :
                        r.Status == ReportStatus.InProgress ? "in_progress" :
                        r.Status == ReportStatus.UnderReview ? "under_review" :
                        r.Status == ReportStatus.Completed ? "completed" :
                        r.Status == ReportStatus.Rejected ? "rejected" : "unknown",

                    StatusAr =
                        r.Status == ReportStatus.Received ? "مستلم" :
                        r.Status == ReportStatus.InProgress ? "قيد التنفيذ" :
                        r.Status == ReportStatus.UnderReview ? "تحت المراجعة" :
                        r.Status == ReportStatus.Completed ? "مكتمل" :
                        r.Status == ReportStatus.Rejected ? "مرفوض" : "غير معروف",

                    AiGarbageDetected = r.AIAnalysisResult != null
                        ? r.AIAnalysisResult.IsGarbageDetected
                        : (bool?)null,

                    AiConfidence = r.AIAnalysisResult != null
                        ? r.AIAnalysisResult.ConfidencePercentage
                        : (decimal?)null,

                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(ct);

            return new PagedResult<ReportAdminListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
