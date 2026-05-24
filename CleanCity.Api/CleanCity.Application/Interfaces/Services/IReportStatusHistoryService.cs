using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IReportStatusHistoryService
    {
        Task<int> CreateAsync(ReportStatusHistory history, CancellationToken ct = default);
        Task<ReportStatusHistory?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<ReportStatusHistory>> GetByReportIdAsync(int reportId, CancellationToken ct = default);
    }
}
