using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface ISewageServiceProviderService
    {
        Task<int> CreateAsync(SewageServiceProvider provider, CancellationToken ct = default);
        Task UpdateAsync(SewageServiceProvider provider, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<SewageServiceProvider?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<SewageServiceProvider>> GetAllAsync(CancellationToken ct = default);
    }
}
