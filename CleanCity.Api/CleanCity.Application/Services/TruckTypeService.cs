using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class TruckTypeService : ITruckTypeService
    {
        private readonly ITruckTypeRepository _types;
        private readonly IUnitOfWork _uow;

        public TruckTypeService(ITruckTypeRepository types, IUnitOfWork uow)
        {
            _types = types;
            _uow = uow;
        }

        public async Task<int> CreateAsync(TruckType type, CancellationToken ct = default)
        {
            await _types.AddAsync(type, ct);
            await _uow.SaveChangesAsync(ct);
            return type.Id;
        }

        public async Task UpdateAsync(TruckType type, CancellationToken ct = default)
        {
            var db = await _types.GetByIdAsync(type.Id, ct);
            if (db == null)
                throw new InvalidOperationException("نوع الشاحنة غير موجود.");

            db.Name = type.Name;

            _types.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _types.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("نوع الشاحنة غير موجود.");

            _types.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<TruckType?> GetByIdAsync(int id, CancellationToken ct = default)
            => _types.GetByIdAsync(id, ct);

        public Task<List<TruckType>> GetAllAsync(CancellationToken ct = default)
            => _types.GetAllAsync(ct);
    }
}
