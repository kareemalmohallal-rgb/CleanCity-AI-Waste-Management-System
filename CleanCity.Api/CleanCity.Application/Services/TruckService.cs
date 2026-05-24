using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class TruckService : ITruckService
    {
        private readonly ITruckRepository _trucks;
        private readonly ITruckTypeRepository _types;
        private readonly IDriverRepository _drivers;
        private readonly IUnitOfWork _uow;

        public TruckService(
            ITruckRepository trucks,
            ITruckTypeRepository types,
            IDriverRepository drivers,
            IUnitOfWork uow)
        {
            _trucks = trucks;
            _types = types;
            _drivers = drivers;
            _uow = uow;
        }

        public async Task<int> CreateAsync(Truck truck, CancellationToken ct = default)
        {
            if (truck.TruckTypeId.HasValue)
            {
                var type = await _types.GetByIdAsync(truck.TruckTypeId.Value, ct);
                if (type == null)
                    throw new InvalidOperationException("نوع الشاحنة غير موجود.");
            }

            if (truck.DriverId.HasValue)
            {
                var driver = await _drivers.GetByIdAsync(truck.DriverId.Value, ct);
                if (driver == null)
                    throw new InvalidOperationException("السائق غير موجود.");
            }

            await _trucks.AddAsync(truck, ct);
            await _uow.SaveChangesAsync(ct);
            return truck.Id;
        }

        public async Task UpdateAsync(Truck truck, CancellationToken ct = default)
        {
            var db = await _trucks.GetByIdAsync(truck.Id, ct);
            if (db == null)
                throw new InvalidOperationException("الشاحنة غير موجودة.");

            if (truck.TruckTypeId.HasValue)
            {
                var type = await _types.GetByIdAsync(truck.TruckTypeId.Value, ct);
                if (type == null)
                    throw new InvalidOperationException("نوع الشاحنة غير موجود.");
            }

            if (truck.DriverId.HasValue)
            {
                var driver = await _drivers.GetByIdAsync(truck.DriverId.Value, ct);
                if (driver == null)
                    throw new InvalidOperationException("السائق غير موجود.");
            }

            db.Name = truck.Name;
            db.Number = truck.Number;
            db.Size = truck.Size;
            db.IsActive = truck.IsActive;
            db.TruckTypeId = truck.TruckTypeId;
            db.DriverId = truck.DriverId;

            _trucks.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _trucks.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("الشاحنة غير موجودة.");

            _trucks.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<Truck?> GetByIdAsync(int id, CancellationToken ct = default)
            => _trucks.GetByIdAsync(id, ct);

        public Task<List<Truck>> GetAllAsync(CancellationToken ct = default)
            => _trucks.GetAllAsync(ct);
    }
}
