using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationContext _context;

        public DriverRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<Driver?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.Drivers
                .Include(x => x.Truck)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<List<Driver>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.Drivers
                .Include(x => x.Area)
                .Include(x => x.Truck)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Driver driver, CancellationToken ct = default)
        {
            await _context.Drivers.AddAsync(driver, ct);
        }

        public void Update(Driver driver)
        {
            _context.Drivers.Update(driver);
        }

        public void Remove(Driver driver)
        {
            _context.Drivers.Remove(driver);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.Drivers
                .AnyAsync(x => x.Id == id, ct);
        }

        public Task<bool> IsActiveAsync(int id, CancellationToken ct = default)
        {
            return _context.Drivers
                .AnyAsync(x => x.Id == id && x.IsActive, ct);
        }
        public async Task<Driver?> GetLeastLoadedActiveByAreaAsync(int areaId, CancellationToken ct)
        {
            // ملاحظة: هذا يختار السائق الأقل عددًا من البلاغات المفتوحة/غير المنتهية.
            // عدّل شروط الحالة حسب حالاتك الفعلية إن كانت أكثر.
            return await _context.Drivers
                .Where(d => d.IsActive && d.AreaId == areaId)
                .OrderBy(d => _context.Reports.Count(r =>
                    r.CurrentDriverId == d.Id &&
                    r.Status != ReportStatus.Completed &&
                    r.Status != ReportStatus.Rejected))
                .ThenBy(d => d.Id) // كسر التعادل
                .FirstOrDefaultAsync(ct);
        }
    }
}
