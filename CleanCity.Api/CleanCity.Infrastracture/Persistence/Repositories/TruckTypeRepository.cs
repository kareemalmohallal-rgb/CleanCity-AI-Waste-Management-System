using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class TruckTypeRepository : ITruckTypeRepository
    {
        private readonly ApplicationContext _context;

        public TruckTypeRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<TruckType>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.TruckTypes
                .OrderBy(x => x.Name)
                .ToListAsync(ct);
        }

        public Task<TruckType?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.TruckTypes
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(TruckType type, CancellationToken ct = default)
        {
            await _context.TruckTypes.AddAsync(type, ct);
        }

        public void Update(TruckType type)
        {
            _context.TruckTypes.Update(type);
        }

        public void Remove(TruckType type)
        {
            _context.TruckTypes.Remove(type);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.TruckTypes.AnyAsync(x => x.Id == id, ct);
        }
    }
}
