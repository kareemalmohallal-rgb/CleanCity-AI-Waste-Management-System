using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Enum;
using CleanCity.Infrastracture.Identity;
using CleanCity.Infrastructure.Data;
using CleanCity.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationContext _db;
    private readonly IReportService _reports;
    private readonly UserManager<ApplicationUser> _userManager;
    public HomeController(ApplicationContext db, IReportService reports, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _reports = reports;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> CreateUserRequest()
    {
        await LoadDriversAsync();
        return View(new CreateUserViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUserRequest(CreateUserViewModel model)
    {
        // مهم جداً: إعادة تحميل القائمة عند الرجوع لنفس الصفحة
        await LoadDriversAsync(model.DriverId);

        // تشخيص سريع أثناء التطوير
        if (!ModelState.IsValid)
        {
            // لو تريد تشوف السبب بالضبط في debug:
            // var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(model);
        }

        // تحقق من وجود السائق
        var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == model.DriverId);
        if (driver == null)
        {
            ModelState.AddModelError(nameof(model.DriverId), "السائق غير موجود");
            return View(model);
        }

        // تحقق من اسم المستخدم
        var existingUser = await _userManager.FindByNameAsync(model.UserName.Trim());
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(model.UserName), "اسم المستخدم مستخدم بالفعل");
            return View(model);
        }

        // تحقق من عدم ربط السائق مسبقاً (إذا لديك DriverId داخل ApplicationUser)
        var userForDriver = await _userManager.Users.FirstOrDefaultAsync(u => u.DriverId == model.DriverId);
        if (userForDriver != null)
        {
            ModelState.AddModelError(nameof(model.DriverId), "هذا السائق مرتبط بمستخدم مسبقاً");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            DriverId = model.DriverId,
            PhoneNumber = driver.PhoneNumber // اختياري
        };

        var createResult = await _userManager.CreateAsync(user, model.Password);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // إضافة دور Driver (اختياري)
        var roleResult = await _userManager.AddToRoleAsync(user, "Driver");
        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
                ModelState.AddModelError(string.Empty, "تم إنشاء المستخدم لكن فشل إضافة الدور: " + error.Description);

            return View(model);
        }

        TempData["Success"] = "تم إنشاء المستخدم بنجاح";

        // إعادة التوجيه لنفس الصفحة (وهذا طبيعي)
        return RedirectToAction(nameof(CreateUserRequest));
    }
    private async Task LoadDriversAsync(int? selectedDriverId = null)
    {
        var drivers = await _db.Drivers
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new
            {
                d.Id,
                d.FullName
            })
            .OrderBy(d => d.FullName)
            .ToListAsync();

        ViewBag.Drivers = new SelectList(drivers, "Id", "FullName", selectedDriverId);
    }
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var todayUtc = now.Date;
        var last24h = now.AddHours(-24);

        // ✅ المناطق للفلتر
        var areas = await _db.Areas
            .OrderBy(a => a.Name)
            .Select(a => new AreaOption { Id = a.Id, Name = a.Name })
            .ToListAsync(ct);

        // ✅ البلاغات اليوم (CreatedAt)
        var reportsToday = await _db.Reports
            .CountAsync(r => r.CreatedAt >= todayUtc, ct);

        // ✅ العالقة: عدّل الحالات حسب Enum عندك
        // هنا اعتبرت أن العالق = Received أو Assigned (غير مكتمل وغير مرفوض)
        var pendingReports = await _db.Reports.CountAsync(r =>
            r.Status != ReportStatus.Completed &&
            r.Status != ReportStatus.Rejected, ct);

        // ✅ تم التنفيذ خلال 24 ساعة
        var completedLast24h = await _db.Reports.CountAsync(r =>
            r.Status == ReportStatus.Completed &&
            r.CreatedAt >= last24h, ct);

        // ✅ متوسط زمن الاستجابة: من CreatedAt إلى AssignedAt (من جدول Assignments)
        // إذا عندك ReportAssignment.AssignedAt مثلما كنت تستخدمه سابقًا
        var avgResponseMinutes = await _db.ReportAssignments
    .Where(a => a.Report.CreatedAt >= last24h)
    .Select(a => (double?)EF.Functions.DateDiffMinute(a.Report.CreatedAt, a.AssignedAt))
    .AverageAsync(ct) ?? 0.0;

        avgResponseMinutes = Math.Round(avgResponseMinutes, 1);

        // ✅ أحدث 5 بلاغات (مع اسم المنطقة والسائق)
        var latestReports = await _db.Reports
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new LatestReportVm
            {
                Id = r.Id,
                AreaName = r.Area != null ? r.Area.Name : "—",
                DriverName = r.CurrentDriver != null ? r.CurrentDriver.FullName : "—",
                CreatedAt = r.CreatedAt,
                StatusText = r.Status.ToString(),
                StatusBadgeClass =
                    r.Status == ReportStatus.Completed ? "badge badge--success" :
                    r.Status == ReportStatus.Rejected ? "badge badge--neutral" :
                    r.Status == ReportStatus.Received ? "badge badge--warn" :
                                                        "badge badge--info"
            })
            .ToListAsync(ct);

        // ✅ أكثر المناطق بلاغاً (آخر 24 ساعة) — Top 3
        var hotspotRaw = await _db.Reports
            .Where(r => r.CreatedAt >= last24h && r.AreaId != null)
            .GroupBy(r => r.Area!.Name)
            .Select(g => new { AreaName = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(3)
            .ToListAsync(ct);

        var max = hotspotRaw.FirstOrDefault()?.Count ?? 0;
        var hotspots = hotspotRaw
            .Select(x => new HotspotVm
            {
                AreaName = x.AreaName,
                Count = x.Count,
                Percent = max == 0 ? 0 : (int)Math.Round((x.Count * 100.0) / max)
            })
            .ToList();

        var vm = new DashboardVm
        {
            ReportsToday = reportsToday,
            PendingReports = pendingReports,
            CompletedLast24h = completedLast24h,
            AvgResponseMinutes = Math.Round(avgResponseMinutes, 1),

            Areas = areas,
            LatestReports = latestReports,
            Hotspots = hotspots
        };

        return View(vm);
    }
    // GET: /Reports/Pending
    [HttpGet]
    public async Task<IActionResult> Pending(CancellationToken ct)
    {
        // 1) مرفوضة من السائق: آخر Assignment حالته Rejected (أو حسب enum عندك)
        var driverRejected = await _db.ReportAssignments
            .Where(a => a != null && a.Status == AssignmentStatus.Rejected) // ✅ من Enum الذي أرسلته
            .OrderByDescending(a => a.ActionAt ?? a.AssignedAt)
            .Select(a => new DriverRejectedReportRow
            {
                ReportId = a.ReportId,
                AreaName = a.Report.Area != null ? a.Report.Area.Name : "—",
                DriverName = a.Driver.FullName,
                RejectionReason = a.RejectionReason,
                RejectedAt = a.ActionAt ?? a.AssignedAt
            })
            .Take(50)
            .ToListAsync(ct);
        var aiRejected = await _db.Reports
            .Where(r => r.Status == ReportStatus.Rejected)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new AiRejectedReportRow
            {
                ReportId = r.Id,
                AreaName = r.Area != null ? r.Area.Name : "—",

                // إذا AIAnalysisResult موجودة وفيها سبب/ثقة، عدّل أسماء الحقول حسب كيانك
                AiReason = r.AIAnalysisResult != null ? r.AIAnalysisResult.RejectionReason : null,
                Confidence = r.AIAnalysisResult != null ? r.AIAnalysisResult.ConfidencePercentage : null,

                // مقترح: السائق الحالي أو أول سائق في المنطقة
                SuggestedDriverName = r.CurrentDriver != null
                    ? r.CurrentDriver.FullName
                    : (r.Area != null && r.Area.Drivers.Any()
                        ? r.Area.Drivers.Select(d => d.FullName).FirstOrDefault() ?? "—"
                        : "—"),

                DecidedAt = r.AIAnalysisResult != null ? r.AIAnalysisResult.AnalyzedAt : (DateTime?)r.CreatedAt
            })
            .Take(50)
            .ToListAsync(ct);

        var vm = new PendingReportsVm
        {
            DriverRejected = driverRejected,
            AiRejected = aiRejected
        };

        return View(vm);
    }
    // GET: /Areas/Hotspots
    [HttpGet]
    public async Task<IActionResult> Hotspots(CancellationToken ct)
    {
        var last24h = DateTime.UtcNow.AddHours(-24);

        var items = await _db.Reports
            .Where(r => r.CreatedAt >= last24h && r.AreaId != null)
            .GroupBy(r => new { r.AreaId, r.Area!.Name })
            .Select(g => new HotspotAreaVm
            {
                AreaId = g.Key.AreaId!.Value,
                AreaName = g.Key.Name,
                ReportsCount = g.Count()
            })
            .OrderByDescending(x => x.ReportsCount)
            .ToListAsync(ct);

        return View(items);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var report = await _db.Reports
            .Include(r => r.Area)
            .Include(r => r.CurrentDriver)
            .Include(r => r.AIAnalysisResult) // إذا عندك كيان لتحليل AI
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (report == null) return NotFound();

        // آخر رفض من السائق (إن وجد)
        var lastRejected = await _db.ReportAssignments
            .Where(a => a.ReportId == id && a.Status == AssignmentStatus.Rejected)
            .OrderByDescending(a => a.ActionAt ?? a.AssignedAt)
            .Include(a => a.Driver)
            .FirstOrDefaultAsync(ct);

        var vm = new ReportDetailsVm
        {
            ReportId = report.Id,
            AreaName = report.Area?.Name ?? "—",
            Description = report.Description ?? "—",
            CreatedAt = report.CreatedAt,
            Status = report.Status.ToString(),
            CurrentDriverName = report.CurrentDriver?.FullName,
            AiReason =  report.AIAnalysisResult?.RejectionReason,
            RejectedByDriverName = lastRejected?.Driver?.FullName,
            RejectionReason = lastRejected?.RejectionReason,
            ImageUrl = report.ImageUrl,
            RejectedAt = lastRejected?.ActionAt ?? lastRejected?.AssignedAt
        };

        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> Reassign(int id, CancellationToken ct)
    {
        var report = await _db.Reports
            .Include(r => r.Area)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (report == null) return NotFound();
        if (report.AreaId == null) return BadRequest("البلاغ بدون منطقة.");

        // السائقون المرتبطون بنفس المنطقة
        var drivers = await _db.Drivers
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reassign(ReassignReportVm vm, CancellationToken ct)
    {
        if (vm.SelectedDriverId == null)
            ModelState.AddModelError(nameof(vm.SelectedDriverId), "اختر سائقًا.");

        if (!ModelState.IsValid)
        {
            vm.Drivers = await _db.Drivers
                .Where(d => d.IsActive && d.AreaId == vm.AreaId)
                .OrderBy(d => d.FullName)
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(d.FullName, d.Id.ToString()))
                .ToListAsync(ct);

            return View(vm);
        }

        // ✅ هذا هو الإسناد الصحيح (نفس منطق النظام)
        await _reports.AssignToDriverAsync(
            reportId: vm.ReportId,
            driverId: vm.SelectedDriverId.Value,
            updatedByType: "Admin",
            updatedByUserId: null,
            ct: ct);

        return RedirectToAction(nameof(Pending));
    }
}