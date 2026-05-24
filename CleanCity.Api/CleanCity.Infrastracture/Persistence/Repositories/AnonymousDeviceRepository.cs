using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using CleanCity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Repositories
{
    public class AnonymousDeviceRepository : IAnonymousDeviceRepository
    {
        private readonly ApplicationContext _context;

        public AnonymousDeviceRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<AnonymousDevice>> GetAllAsync(CancellationToken ct)
        {
            return await _context.AnonymousDevices
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<AnonymousDevice?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.AnonymousDevices
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<AnonymousDevice?> GetByDeviceIdentifierAsync(
            string deviceIdentifier,
            CancellationToken ct)
        {
            return await _context.AnonymousDevices
                .FirstOrDefaultAsync(x => x.DeviceIdentifier == deviceIdentifier, ct);
        }

        public async Task<int> CreateAsync(AnonymousDevice device, CancellationToken ct)
        {
            await _context.AnonymousDevices.AddAsync(device, ct);
            await _context.SaveChangesAsync(ct);
            return device.Id;
        }

        public async Task UpdateAsync(AnonymousDevice device, CancellationToken ct)
        {
            _context.AnonymousDevices.Update(device);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _context.AnonymousDevices
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity == null)
                throw new InvalidOperationException("AnonymousDevice not found.");

            _context.AnonymousDevices.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}