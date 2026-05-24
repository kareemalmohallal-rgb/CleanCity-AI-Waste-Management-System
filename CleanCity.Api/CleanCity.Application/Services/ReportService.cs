using CleanCity.Application.Common;
using CleanCity.Application.DTOs.Reports;
using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;

namespace CleanCity.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reports;
        private readonly IDriverRepository _drivers;
        private readonly IAnonymousDeviceRepository _devices;
        private readonly IReportAssignmentRepository _assignments;
        private readonly IReportStatusHistoryRepository _history;
        private readonly IAreaRepository _areaRepo;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _uow;
        private readonly IWasteImageValidationService _aiValidator;
        private readonly IAIAnalysisResultRepository _aiResults;
        private readonly IPushNotificationSender _pushSender;
        private readonly INotificationService _notificationService;

        public ReportService(
            IReportRepository reports,
            IDriverRepository drivers,
            IAnonymousDeviceRepository devices,
            IReportAssignmentRepository assignments,
            IReportStatusHistoryRepository history,
            IAreaRepository areaRepo,
            IFileService fileService,
            IUnitOfWork uow,
            IWasteImageValidationService aiValidator,
            IAIAnalysisResultRepository aiResults,
            IPushNotificationSender pushSender,
            INotificationService notificationService)
        {
            _reports = reports;
            _drivers = drivers;
            _devices = devices;
            _assignments = assignments;
            _history = history;
            _areaRepo = areaRepo;
            _fileService = fileService;
            _uow = uow;
            _aiValidator = aiValidator;
            _aiResults = aiResults;
            _pushSender = pushSender;
            _notificationService = notificationService;
        }

        // ─── إنشاء بلاغ جديد ──────────────────────────────────────────────────

        public async Task<int> CreateAsync(CreateReportDto dto, CancellationToken ct = default)
        {
            var latitude = dto.Latitude;
            var longitude = dto.Longitude;

            var area = await _areaRepo.FindDistrictAsync(latitude, longitude, ct);
            if (area is null)
                throw new Exception("الموقع خارج نطاق الخدمة.");

            var driver = await _drivers.GetLeastLoadedActiveByAreaAsync(area.Id, ct);
            if (driver is null)
                throw new InvalidOperationException("لا يوجد سائق فعّال لهذه المنطقة.");

            if (dto.ImageStream == null || string.IsNullOrWhiteSpace(dto.ImageFileName))
                throw new InvalidOperationException("صورة البلاغ مطلوبة للتحليل.");

            var report = new Report
            {
                ImageUrl = string.Empty,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Description = dto.Description,
                AnonymousDeviceId = dto.AnonymousDeviceId,
                AreaId = area.Id,
                CurrentDriverId = null,
                Status = ReportStatus.Received,
                CreatedAt = DateTime.UtcNow
            };

            await _reports.AddAsync(report, ct);
            await _uow.SaveChangesAsync(ct);

            await using var buffer = new MemoryStream();
            await dto.ImageStream.CopyToAsync(buffer, ct);
            var imageBytes = buffer.ToArray();

            await using (var saveStream = new MemoryStream(imageBytes))
            {
                var (_, stored) = await _fileService.SaveReportImageAsync(
                    reportId: report.Id,
                    fileStream: saveStream,
                    originalFileName: dto.ImageFileName!,
                    contentType: dto.ImageContentType ?? "application/octet-stream",
                    ct: ct);

                report.ImageUrl = stored.PublicUrl;
            }

            var aiResult = default(CleanCity.Application.DTOs.AI.WasteImageValidationResult);

            await using (var aiStream = new MemoryStream(imageBytes))
            {
                aiResult = await _aiValidator.ValidateAsync(
                    aiStream,
                    dto.ImageFileName!,
                    dto.ImageContentType ?? "application/octet-stream",
                    ct);
            }

            if (aiResult == null)
                throw new InvalidOperationException("تعذر الحصول على نتيجة التحليل من خدمة الذكاء الاصطناعي.");

            await _aiResults.AddAsync(new AIAnalysisResult
            {
                ReportId = report.Id,
                IsGarbageDetected = aiResult.IsGarbageDetected,
                ConfidencePercentage = aiResult.ConfidencePercentage,
                RejectionReason = aiResult.RejectionReason,
                AnalyzedAt = DateTime.UtcNow
            }, ct);

            var oldStatus = report.Status;

            if (aiResult.IsGarbageDetected)
            {
                report.Status = ReportStatus.Received;
                report.CurrentDriverId = driver.Id;
            }
            else
            {
                report.Status = ReportStatus.Rejected;
                report.CurrentDriverId = null;
            }

            await _history.AddAsync(new ReportStatusHistory
            {
                ReportId = report.Id,
                OldStatus = oldStatus,
                NewStatus = report.Status,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByType = "System",
                UpdatedByUserId = null
            }, ct);

            await _uow.SaveChangesAsync(ct);

            // ✅ إشعار 1 — المستخدم المجهول: تم استلام البلاغ
            await NotifyAnonymousDeviceAsync(
                anonymousDeviceId: report.AnonymousDeviceId,
                reportId: report.Id,
                type: NotificationType.Report,
                title: "تم استلام بلاغك ✅",
                body: aiResult.IsGarbageDetected
                    ? "تم استلام بلاغك بنجاح وسيتم إسناده لسائق قريباً."
                    : "عذراً، لم يتم قبول بلاغك — الصورة لا تحتوي على نفايات.",
                fcmType: "report_received",
                ct: ct);

            if (report.Status == ReportStatus.Received)
            {
                await AssignToDriverAsync(
                    reportId: report.Id,
                    driverId: driver.Id,
                    updatedByType: "System",
                    updatedByUserId: null,
                    ct: ct);
            }

            await _uow.SaveChangesAsync(ct);
            return report.Id;
        }

        // ─── إسناد بلاغ لسائق ─────────────────────────────────────────────────

        public async Task AssignToDriverAsync(
            int reportId,
            int driverId,
            string updatedByType,
            string? updatedByUserId,
            CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            var driver = await _drivers.GetByIdAsync(driverId, ct);
            if (driver == null || !driver.IsActive)
                throw new InvalidOperationException("السائق غير موجود أو غير نشط.");

            var assignment = new ReportAssignment
            {
                ReportId = reportId,
                DriverId = driverId,
                Status = AssignmentStatus.Assigned,
                AssignedAt = DateTime.UtcNow,
                ActionAt = null,
                RejectionReason = null
            };

            await _assignments.AddAsync(assignment, ct);

            report.CurrentDriverId = driverId;

            var old = report.Status;
            report.Status = ReportStatus.Received;

            await _history.AddAsync(new ReportStatusHistory
            {
                ReportId = reportId,
                OldStatus = old,
                NewStatus = report.Status,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByType = updatedByType,
                UpdatedByUserId = updatedByUserId
            }, ct);

            await _uow.SaveChangesAsync(ct);

            // ✅ إشعار للسائق (موجود سابقاً — لا تغيير)
            var driverNotification = new Notification
            {
                Type = NotificationType.Assignment,
                Title = "تم إسناد بلاغ جديد",
                Body = "لديك بلاغ جديد في منطقتك. افتح التطبيق لمراجعة التفاصيل.",
                DeepLink = $"/reports/{reportId}",
                CreatedAt = DateTime.UtcNow,
                ReportAssignmentId = assignment.Id
            };

            await _notificationService.CreateAsync(
                driverNotification,
                new List<NotificationRecipient>
                {
                    new NotificationRecipient { DriverId = driverId, IsRead = false }
                },
                ct);

            if (!string.IsNullOrWhiteSpace(driver.FcmToken))
            {
                await _pushSender.SendToTokenAsync(
                    token: driver.FcmToken,
                    title: "تم إسناد بلاغ جديد",
                    body: "لديك بلاغ جديد في منطقتك. افتح التطبيق لمراجعة التفاصيل.",
                    data: new Dictionary<string, string>
                    {
                        ["type"] = "driver_assignment",
                        ["reportId"] = reportId.ToString()
                    },
                    ct: ct);
            }

            // ✅ إشعار 2 — المستخدم المجهول: تم إسناد بلاغك لسائق
            await NotifyAnonymousDeviceAsync(
                anonymousDeviceId: report.AnonymousDeviceId,
                reportId: reportId,
                type: NotificationType.Report,
                title: "جارٍ معالجة بلاغك 🚗",
                body: "تم إسناد بلاغك لسائق وسيصل الى الموقع قريباً.",
                fcmType: "report_assigned",
                ct: ct);
        }

        // ─── تحديث حالة البلاغ ────────────────────────────────────────────────

        public async Task UpdateStatusAsync(int reportId, UpdateReportStatusDto dto, CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            var old = report.Status;
            report.Status = dto.NewStatus;

            await _history.AddAsync(new ReportStatusHistory
            {
                ReportId = reportId,
                OldStatus = old,
                NewStatus = dto.NewStatus,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByType = dto.UpdatedByType,
                UpdatedByUserId = dto.UpdatedByUserId
            }, ct);

            await _uow.SaveChangesAsync(ct);
        }

        // ─── قبول السائق للبلاغ ───────────────────────────────────────────────

        public async Task DriverAcceptAsync(int reportId, int driverId, CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            var assignment = await _assignments.GetCurrentAssignedAsync(reportId, driverId, ct);
            if (assignment == null)
                throw new InvalidOperationException("لا يوجد إسناد نشط لهذا البلاغ مع هذا السائق.");

            if (assignment.Status != AssignmentStatus.Assigned)
                throw new InvalidOperationException("تمت معالجة هذا الإسناد مسبقاً.");

            var oldReportStatus = report.Status;

            assignment.Status = AssignmentStatus.Completed;
            assignment.ActionAt = DateTime.UtcNow;
            assignment.RejectionReason = null;

            report.Status = ReportStatus.InProgress;
            report.CurrentDriverId = driverId;

            await _history.AddAsync(new ReportStatusHistory
            {
                ReportId = report.Id,
                OldStatus = oldReportStatus,
                NewStatus = ReportStatus.InProgress,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByType = "Driver",
                UpdatedByUserId = driverId.ToString()
            }, ct);

            await _uow.SaveChangesAsync(ct);
        }

        // ─── رفض السائق للبلاغ ────────────────────────────────────────────────

        public async Task DriverRejectAsync(
            int reportId,
            int driverId,
            string? rejectionReason,
            CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            var assignment = await _assignments.GetCurrentAssignedAsync(reportId, driverId, ct);
            if (assignment == null)
                throw new InvalidOperationException("لا يوجد إسناد نشط لهذا البلاغ مع هذا السائق.");

            if (assignment.Status != AssignmentStatus.Assigned)
                throw new InvalidOperationException("تمت معالجة هذا الإسناد مسبقاً.");

            var oldReportStatus = report.Status;

            assignment.Status = AssignmentStatus.Rejected;
            assignment.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason)
                ? null
                : rejectionReason.Trim();
            assignment.ActionAt = DateTime.UtcNow;

            report.Status = ReportStatus.Received;
            report.CurrentDriverId = null;

            await _history.AddAsync(new ReportStatusHistory
            {
                ReportId = report.Id,
                OldStatus = oldReportStatus,
                NewStatus = ReportStatus.Received,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByType = "Driver",
                UpdatedByUserId = driverId.ToString()
            }, ct);

            await _uow.SaveChangesAsync(ct);
        }

        // ─── استعلام ──────────────────────────────────────────────────────────

        public async Task<Report?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _reports.GetByIdAsync(id, ct);
        }

        public async Task<List<DriverReportListItemDto>> GetReportsByDriverIdAsync(int id, CancellationToken ct = default)
        {
            return await _reports.GetReportsByDriverIdAsync(id, ct);
        }

        // ─── إنهاء السائق للبلاغ ──────────────────────────────────────────────

        public async Task DriverCompleteExecutionAsync(
            int reportId,
            int driverId,
            ReportStatus finalStatus,
            CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            if (report.CurrentDriverId != driverId)
                throw new InvalidOperationException("هذا البلاغ ليس تابعاً لهذا السائق.");

            if (report.Status != ReportStatus.InProgress)
                throw new InvalidOperationException("لا يمكن إنهاء البلاغ لأنه ليس في حالة قيد التنفيذ.");

            if (finalStatus != ReportStatus.Completed && finalStatus != ReportStatus.UnderReview)
                throw new InvalidOperationException("الحالة النهائية المسموحة هي Completed أو UnderReview فقط.");

            var oldStatus = report.Status;
            report.Status = finalStatus;

            await _history.AddAsync(new ReportStatusHistory
            {
                ReportId = report.Id,
                OldStatus = oldStatus,
                NewStatus = finalStatus,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByType = "Driver",
                UpdatedByUserId = driverId.ToString()
            }, ct);

            await _uow.SaveChangesAsync(ct);

            // ✅ إشعار 3 — المستخدم المجهول: تم تنفيذ البلاغ
            var (title, body) = finalStatus == ReportStatus.Completed
                ? ("تم تنفيذ بلاغك ✅", "تم الانتهاء من معالجة بلاغك بنجاح. شكراً لمساهمتك في نظافة المدينة!")
                : ("بلاغك قيد المراجعة 🔍", "تم رفع بلاغك للمراجعة وسيتم إخطارك بالنتيجة قريباً.");

            await NotifyAnonymousDeviceAsync(
                anonymousDeviceId: report.AnonymousDeviceId,
                reportId: reportId,
                type: NotificationType.Report,
                title: title,
                body: body,
                fcmType: "report_completed",
                ct: ct);
        }

        // ─── قائمة البلاغات لصفحة الإدارة (مُرشَّحة + مُقسَّمة) ──────────────

        public async Task<PagedResult<ReportAdminListItemDto>> GetPagedAsync(
            ReportFilterDto filter,
            CancellationToken ct = default)
        {
            return await _reports.GetPagedAsync(filter, ct);
        }

        // ══════════════════════════════════════════════════════════════════════
        // ─── Helper: إرسال إشعار للمستخدم المجهول ────────────────────────────
        // يجلب الجهاز، يحفظ الإشعار في DB، ويرسل FCM إن وُجد Token
        // ══════════════════════════════════════════════════════════════════════
        private async Task NotifyAnonymousDeviceAsync(
            int anonymousDeviceId,
            int reportId,
            NotificationType type,
            string title,
            string body,
            string fcmType,
            CancellationToken ct)
        {
            // 1) جلب الجهاز من قاعدة البيانات
            var device = await _devices.GetByIdAsync(anonymousDeviceId, ct);
            if (device == null) return;

            // 2) حفظ الإشعار في DB
            var notification = new Notification
            {
                Type = type,
                Title = title,
                Body = body,
                DeepLink = $"/reports/{reportId}",
                CreatedAt = DateTime.UtcNow,
                ReportId = reportId
            };

            await _notificationService.CreateAsync(
                notification,
                new List<NotificationRecipient>
                {
                    new NotificationRecipient
                    {
                        AnonymousDeviceId = anonymousDeviceId,
                        IsRead = false
                    }
                },
                ct);

            // 3) إرسال FCM إن وُجد Token
            if (!string.IsNullOrWhiteSpace(device.FcmToken))
            {
                await _pushSender.SendToTokenAsync(
                    token: device.FcmToken,
                    title: title,
                    body: body,
                    data: new Dictionary<string, string>
                    {
                        ["type"] = fcmType,
                        ["reportId"] = reportId.ToString()
                    },
                    ct: ct);
            }
        }
    }
}