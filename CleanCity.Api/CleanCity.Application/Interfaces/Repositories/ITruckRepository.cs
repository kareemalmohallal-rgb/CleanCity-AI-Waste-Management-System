using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface ITruckRepository
    {
        Task<List<Truck>> GetAllAsync(CancellationToken ct = default);
        Task<Truck?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Truck truck, CancellationToken ct = default);
        void Update(Truck truck);
        void Remove(Truck truck);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
