using CleanCity.Application.DTOs.AreaListItem;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationContext _context;

        public AreaRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<Area?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.Areas
                .Include(a => a.Reports)
                .FirstOrDefaultAsync(a => a.Id == id, ct);
        }
        public Task<List<Area>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.Areas
                .ToListAsync(ct);
        }
        public Task<List<AreaListItemDto>> GetAllForIndexAsync(CancellationToken ct = default)
        {
            return _context.Areas
                .Select(a => new AreaListItemDto
                {
                    Id = a.Id,
                    Name = a.Name,

                    // ✅ عدد البلاغات بدون Include
                    ReportsCount = a.Reports.Count(),

                    // ✅ اسم السائق المسؤول (إذا عندك أكثر من سائق سيأخذ أول واحد)
                    ResponsibleDriverName = a.Drivers
                        .Where(d => d.IsActive)
                        .Select(d => d.FullName)
                        .FirstOrDefault()
                })
                .ToListAsync(ct);
        }
        public async Task AddAsync(Area area, CancellationToken ct = default)
        {
            await _context.Areas.AddAsync(area, ct);
        }

        public void Update(Area area)
        {
            _context.Areas.Update(area);
        }

        public void Remove(Area area)
        {
            _context.Areas.Remove(area);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.Areas.AnyAsync(x => x.Id == id, ct);
        }
        public async Task<Area?> FindDistrictAsync(double Latitude, double Longitude, CancellationToken ct)
        {
            return await _context.Areas
                .Include(a => a.Drivers.Where(d => d.IsActive))
        .Where(a => Latitude >= a.MinLat && Latitude <= a.MaxLat
                 && Longitude >= a.MinLng && Longitude <= a.MaxLng)
        .SingleOrDefaultAsync(ct); // استخدم First إذا في تداخل
        }
    }
}
