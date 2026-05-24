using CleanCity.Application.DTOs.AreaListItem;
using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areas;
        private readonly IUnitOfWork _uow;

        public AreaService(IAreaRepository areas, IUnitOfWork uow)
        {
            _areas = areas;
            _uow = uow;
        }

        public async Task<int> CreateAsync(Area area, CancellationToken ct = default)
        {
            await _areas.AddAsync(area, ct);
            await _uow.SaveChangesAsync(ct);
            return area.Id;
        }

        public async Task UpdateAsync(Area area, CancellationToken ct = default)
        {
            var db = await _areas.GetByIdAsync(area.Id, ct);
            if (db == null)
                throw new InvalidOperationException("المنطقة غير موجودة.");

            db.Name = area.Name;
            db.MinLat = area.MinLat;
            db.MaxLat = area.MaxLat;
            db.MinLng = area.MinLng;
            db.MaxLng = area.MaxLng;
            _areas.Update(db);
            await _uow.SaveChangesAsync(ct);
        }
        public Task<List<AreaListItemDto>> GetAllForIndexAsync(CancellationToken ct = default)
        {
            return _areas.GetAllForIndexAsync(ct);
        }
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _areas.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("المنطقة غير موجودة.");

            _areas.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<Area?> GetByIdAsync(int id, CancellationToken ct = default)
            => _areas.GetByIdAsync(id, ct);

        public Task<List<Area>> GetAllAsync(CancellationToken ct = default)
            => _areas.GetAllAsync(ct);
    }
}
