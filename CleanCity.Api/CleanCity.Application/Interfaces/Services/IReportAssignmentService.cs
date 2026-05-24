using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IReportAssignmentService
    {
        Task<int> CreateAsync(ReportAssignment assignment, CancellationToken ct = default);
        Task UpdateAsync(ReportAssignment assignment, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<ReportAssignment?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<ReportAssignment>> GetAllAsync(CancellationToken ct = default);
        Task<List<ReportAssignment>> GetByReportIdAsync(int reportId, CancellationToken ct = default);
        Task AcceptByDriverAsync(int reportId, int driverId, CancellationToken ct = default);

        Task RejectByDriverAsync(
            int reportId,
            int driverId,
            string? rejectionReason,
            CancellationToken ct = default);

    }
}
