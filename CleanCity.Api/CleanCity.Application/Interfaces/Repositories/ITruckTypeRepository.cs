using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface ITruckTypeRepository
    {
        Task<List<TruckType>> GetAllAsync(CancellationToken ct = default);
        Task<TruckType?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(TruckType type, CancellationToken ct = default);
        void Update(TruckType type);
        void Remove(TruckType type);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
