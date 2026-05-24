using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class SewageServiceProviderRepository : ISewageServiceProviderRepository
    {
        private readonly ApplicationContext _context;

        public SewageServiceProviderRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<SewageServiceProvider>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.SewageServiceProviders
                .OrderBy(x => x.OwnerName)
                .ToListAsync(ct);
        }

        public Task<SewageServiceProvider?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.SewageServiceProviders
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(SewageServiceProvider item, CancellationToken ct = default)
        {
            await _context.SewageServiceProviders.AddAsync(item, ct);
        }

        public void Update(SewageServiceProvider item)
        {
            _context.SewageServiceProviders.Update(item);
        }

        public void Remove(SewageServiceProvider item)
        {
            _context.SewageServiceProviders.Remove(item);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.SewageServiceProviders.AnyAsync(x => x.Id == id, ct);
        }
    }
}
