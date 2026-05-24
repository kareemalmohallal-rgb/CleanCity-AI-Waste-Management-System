using CleanCity.Application.DTOs.Reports;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Enum;
using CleanCity.Infrastructure.Data;
using CleanCity.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.MVC.Controllers
{
    /// <summary>
    /// صفحة إدارة البلاغات: عرض القائمة، الفلترة، التفاصيل، وإعادة الإسناد.
    /// يتبع Clean Architecture: Controller → Service → Repository → DbContext.
    /// </summary>
    //[Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IAreaService _areaService;
        private readonly ApplicationContext _db; // للحصول على قوائم المناطق فقط

        public ReportsController(
            IReportService reportService,
            IAreaService areaService,
            ApplicationContext db)
        {
            _reportService = reportService;
            _areaService = areaService;
            _db = db;
        }

        // ── GET /Reports ─────────────────────────────────────────────────────
        /// <summary>
        /// الصفحة الرئيسية: قائمة البلاغات مع دعم الفلترة والتصفح.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            ReportStatus? status,
            int? areaId,
            DateTime? dateFrom,
            DateTime? dateTo,
            string? search,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            // ── بناء DTO الفلتر ──────────────────────────────────────────────
            var filter = new ReportFilterDto
            {
                Status = status,
                AreaId = areaId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Search = search,
                Page = page,
                PageSize = pageSize
            };

            // ── الاستعلام عبر Service ────────────────────────────────────────
            var paged = await _reportService.GetPagedAsync(filter, ct);

            // ── تحميل المناطق لقائمة الفلتر ──────────────────────────────────
            var areas = await _db.Areas
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem(a.Name, a.Id.ToString()))
                .ToListAsync(ct);

            areas.Insert(0, new SelectListItem("كل المناطق", ""));

            // ── تجميع ViewModel ──────────────────────────────────────────────
            var vm = new ReportListVm
            {
                Paged = paged,
                FilterStatus = status,
                FilterAreaId = areaId,
                FilterDateFrom = dateFrom,
                FilterDateTo = dateTo,
                FilterSearch = search,
                CurrentPage = page,
                PageSize = pageSize,
                AreaOptions = areas
            };

            return View(vm);
        }

        // ── GET /Reports/Details/5 ────────────────────────────────────────────
        /// <summary>
        /// تفاصيل بلاغ واحد.
        /// يُعيد استخدام ReportDetailsVm الموجودة في المشروع.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var report = await _db.Reports
                .AsNoTracking()
                .Include(r => r.Area)
                .Include(r => r.CurrentDriver)
                .Include(r => r.AIAnalysisResult)
                .Include(r => r.StatusHistories!.OrderByDescending(h => h.UpdatedAt).Take(10))
                .Include(r => r.Assignments!)
                    .ThenInclude(a => a.Driver)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (report is null) return NotFound();

            // آخر رفض من السائق
            var lastRejection = report.Assignments?
                .Where(a => a.Status == Domain.Enum.AssignmentStatus.Rejected)
                .OrderByDescending(a => a.ActionAt ?? a.AssignedAt)
                .FirstOrDefault();

            var vm = new ReportDetailsVm
            {
                ReportId = report.Id,
                AreaName = report.Area?.Name ?? "—",
                Description = report.Description ?? "—",
                CreatedAt = report.CreatedAt,
                Status = report.Status.ToString(),
                CurrentDriverName = report.CurrentDriver?.FullName,
                AiReason = report.AIAnalysisResult?.RejectionReason,
                RejectedByDriverName = lastRejection?.Driver?.FullName,
                RejectionReason = lastRejection?.RejectionReason,
                ImageUrl = report.ImageUrl,
                RejectedAt = lastRejection?.ActionAt ?? lastRejection?.AssignedAt
            };

            return View(vm);
        }

        // ── GET /Reports/Reassign/5 ───────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Reassign(int id, CancellationToken ct)
        {
            var report = await _db.Reports
                .AsNoTracking()
                .Include(r => r.Area)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (report is null) return NotFound();
            if (report.AreaId is null) return BadRequest("البلاغ ليس له منطقة محددة.");

            var drivers = await _db.Drivers
                .AsNoTracking()
                .Where(d => d.IsActive && d.AreaId == report.AreaId)
                .OrderBy(d => d.FullName)
                .Select(d => new SelectListItem(d.FullName, d.Id.ToString()))
                .ToListAsync(ct);

            var vm = new ReassignReportVm
            {
                ReportId = report.Id,
                AreaId = report.AreaId.Value,
                AreaName = report.Area?.Name ?? "—",
                Drivers = drivers
            };

            return View(vm);
        }

        // ── POST /Reports/Reassign ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reassign(ReassignReportVm vm, CancellationToken ct)
        {
            if (vm.SelectedDriverId is null)
                ModelState.AddModelError(nameof(vm.SelectedDriverId), "يرجى اختيار سائق.");

            if (!ModelState.IsValid)
            {
                vm.Drivers = await _db.Drivers
                    .AsNoTracking()
                    .Where(d => d.IsActive && d.AreaId == vm.AreaId)
                    .OrderBy(d => d.FullName)
                    .Select(d => new SelectListItem(d.FullName, d.Id.ToString()))
                    .ToListAsync(ct);

                return View(vm);
            }

            await _reportService.AssignToDriverAsync(
                reportId: vm.ReportId,
                driverId: vm.SelectedDriverId!.Value,
                updatedByType: "Admin",
                updatedByUserId: null,
                ct: ct);

            TempData["Success"] = $"تم إسناد البلاغ #{vm.ReportId} بنجاح.";
            return RedirectToAction(nameof(Index));
        }
    }
}