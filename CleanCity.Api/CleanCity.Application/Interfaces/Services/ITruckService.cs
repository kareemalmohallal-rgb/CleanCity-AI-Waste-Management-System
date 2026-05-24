using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface ITruckService
    {
        Task<int> CreateAsync(Truck truck, CancellationToken ct = default);
        Task UpdateAsync(Truck truck, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<Truck?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<Truck>> GetAllAsync(CancellationToken ct = default);
    }
}
