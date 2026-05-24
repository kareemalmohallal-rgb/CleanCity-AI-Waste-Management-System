using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class ReportStatusHistoryService : IReportStatusHistoryService
    {
        private readonly IReportStatusHistoryRepository _history;
        private readonly IReportRepository _reports;
        private readonly IUnitOfWork _uow;

        public ReportStatusHistoryService(
            IReportStatusHistoryRepository history,
            IReportRepository reports,
            IUnitOfWork uow)
        {
            _history = history;
            _reports = reports;
            _uow = uow;
        }

        public async Task<int> CreateAsync(ReportStatusHistory history, CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(history.ReportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            await _history.AddAsync(history, ct);
            await _uow.SaveChangesAsync(ct);
            return history.Id;
        }

        public Task<ReportStatusHistory?> GetByIdAsync(int id, CancellationToken ct = default)
            => _history.GetByIdAsync(id, ct);

        public Task<List<ReportStatusHistory>> GetByReportIdAsync(int reportId, CancellationToken ct = default)
            => _history.GetByReportIdAsync(reportId, ct);
    }
}
