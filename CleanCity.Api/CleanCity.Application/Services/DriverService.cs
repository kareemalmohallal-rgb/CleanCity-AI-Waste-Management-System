using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _drivers;
        private readonly IAreaRepository _areas;
        private readonly IUnitOfWork _uow;

        public DriverService(
            IDriverRepository drivers,
            IAreaRepository areas,
            IUnitOfWork uow)
        {
            _drivers = drivers;
            _areas = areas;
            _uow = uow;
        }

        public async Task<int> CreateAsync(Driver driver, CancellationToken ct = default)
        {
            if (driver.AreaId.HasValue)
            {
                var area = await _areas.GetByIdAsync(driver.AreaId.Value, ct);
                if (area == null)
                    throw new InvalidOperationException("المنطقة المرتبطة بالسائق غير موجودة.");
            }

            await _drivers.AddAsync(driver, ct);
            await _uow.SaveChangesAsync(ct);
            return driver.Id;
        }

        public async Task UpdateAsync(Driver driver, CancellationToken ct = default)
        {
            var db = await _drivers.GetByIdAsync(driver.Id, ct);
            if (db == null)
                throw new InvalidOperationException("السائق غير موجود.");

            if (driver.AreaId.HasValue)
            {
                var area = await _areas.GetByIdAsync(driver.AreaId.Value, ct);
                if (area == null)
                    throw new InvalidOperationException("المنطقة غير موجودة.");
            }

            db.FullName = driver.FullName;
            db.PhoneNumber = driver.PhoneNumber;
            db.LicenseNumber = driver.LicenseNumber;
            db.IsActive = driver.IsActive;
            db.AreaId = driver.AreaId;

            _drivers.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _drivers.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("السائق غير موجود.");

            _drivers.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<Driver?> GetByIdAsync(int id, CancellationToken ct = default)
            => _drivers.GetByIdAsync(id, ct);

        public Task<List<Driver>> GetAllAsync(CancellationToken ct = default)
            => _drivers.GetAllAsync(ct);
    }
}
