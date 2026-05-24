using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class AwarenessContentRepository : IAwarenessContentRepository
    {
        private readonly ApplicationContext _context;

        public AwarenessContentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<AwarenessContent>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.AwarenessContents
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }

        public Task<AwarenessContent?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.AwarenessContents
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(AwarenessContent item, CancellationToken ct = default)
        {
            await _context.AwarenessContents.AddAsync(item, ct);
        }

        public void Update(AwarenessContent item)
        {
            _context.AwarenessContents.Update(item);
        }

        public void Remove(AwarenessContent item)
        {
            _context.AwarenessContents.Remove(item);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.AwarenessContents
                .AnyAsync(x => x.Id == id, ct);
        }
    }
}
