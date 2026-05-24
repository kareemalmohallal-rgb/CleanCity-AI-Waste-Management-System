using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;

namespace CleanCity.Application.Services
{
    public class ReportAssignmentService : IReportAssignmentService
    {
        private readonly IReportAssignmentRepository _assignments;
        private readonly IReportRepository _reports;
        private readonly IDriverRepository _drivers;
        private readonly IUnitOfWork _uow;

        public ReportAssignmentService(
            IReportAssignmentRepository assignments,
            IReportRepository reports,
            IDriverRepository drivers,
            IUnitOfWork uow)
        {
            _assignments = assignments;
            _reports = reports;
            _drivers = drivers;
            _uow = uow;
        }

        public async Task<int> CreateAsync(ReportAssignment assignment, CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(assignment.ReportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            var driver = await _drivers.GetByIdAsync(assignment.DriverId, ct);
            if (driver == null)
                throw new InvalidOperationException("السائق غير موجود.");

            assignment.Status = AssignmentStatus.Assigned;
            assignment.AssignedAt = DateTime.UtcNow;
            assignment.ActionAt = null;

            await _assignments.AddAsync(assignment, ct);
            await _uow.SaveChangesAsync(ct);
            return assignment.Id;
        }

        public async Task UpdateAsync(ReportAssignment assignment, CancellationToken ct = default)
        {
            var db = await _assignments.GetByIdAsync(assignment.Id, ct);
            if (db == null)
                throw new InvalidOperationException("الإسناد غير موجود.");

            db.Status = assignment.Status;
            db.RejectionReason = assignment.RejectionReason;
            db.ActionAt = assignment.ActionAt;

            _assignments.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _assignments.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("الإسناد غير موجود.");

            _assignments.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<ReportAssignment?> GetByIdAsync(int id, CancellationToken ct = default)
            => _assignments.GetByIdAsync(id, ct);

        public Task<List<ReportAssignment>> GetAllAsync(CancellationToken ct = default)
            => _assignments.GetAllAsync(ct);

        public Task<List<ReportAssignment>> GetByReportIdAsync(int reportId, CancellationToken ct = default)
            => _assignments.GetByReportIdAsync(reportId, ct);

        public async Task AcceptByDriverAsync(int reportId, int driverId, CancellationToken ct = default)
        {
            var assignment = await _assignments.GetCurrentAssignedAsync(reportId, driverId, ct);
            if (assignment == null)
                throw new InvalidOperationException("لا يوجد إسناد نشط لهذا البلاغ مع هذا السائق.");

            var report = assignment.Report ?? await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            if (assignment.Status != AssignmentStatus.Assigned)
                throw new InvalidOperationException("هذا الإسناد تمت معالجته مسبقًا.");

            assignment.Status = AssignmentStatus.Completed;
            assignment.ActionAt = DateTime.UtcNow;
            assignment.RejectionReason = null;

            report.Status = ReportStatus.InProgress;
            report.CurrentDriverId = driverId;

            await _uow.SaveChangesAsync(ct);
        }

        public async Task RejectByDriverAsync(
            int reportId,
            int driverId,
            string? rejectionReason,
            CancellationToken ct = default)
        {
            var assignment = await _assignments.GetCurrentAssignedAsync(reportId, driverId, ct);
            if (assignment == null)
                throw new InvalidOperationException("لا يوجد إسناد نشط لهذا البلاغ مع هذا السائق.");

            var report = assignment.Report ?? await _reports.GetByIdAsync(reportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            if (assignment.Status != AssignmentStatus.Assigned)
                throw new InvalidOperationException("هذا الإسناد تمت معالجته مسبقًا.");

            assignment.Status = AssignmentStatus.Rejected;
            assignment.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason)
                ? null
                : rejectionReason.Trim();
            assignment.ActionAt = DateTime.UtcNow;

            report.Status = ReportStatus.UnderReview;

            if (report.CurrentDriverId == driverId)
            {
                report.CurrentDriverId = null;
            }

            await _uow.SaveChangesAsync(ct);
        }
    }
}