using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IAwarenessContentService
    {
        Task<int> CreateAsync(AwarenessContent content, CancellationToken ct = default);
        Task UpdateAsync(AwarenessContent content, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<AwarenessContent?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<AwarenessContent>> GetAllAsync(CancellationToken ct = default);
    }
}
