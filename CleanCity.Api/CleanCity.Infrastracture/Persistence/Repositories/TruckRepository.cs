//using CleanCity.Application.Interfaces.Repositories;
//using CleanCity.Domain.Entities;
//using CleanCity.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;

//namespace CleanCity.Infrastructure.Persistence.Repositories
//{
//    public class TruckRepository : ITruckRepository
//    {
//        private readonly ApplicationContext _context;

//        public TruckRepository(ApplicationContext context)
//        {
//            _context = context;
//        }

//        public Task<List<Truck>> GetAllAsync(CancellationToken ct = default)
//        {
//            return _context.Trucks
//                .Include(x => x.TruckType)
//                .Include(x => x.Driver)
//                .ToListAsync(ct);
//        }

//        public Task<Truck?> GetByIdAsync(int id, CancellationToken ct = default)
//        {
//            return _context.Trucks
//                .Include(x => x.TruckType)
//                .Include(x => x.Driver)
//                .FirstOrDefaultAsync(x => x.Id == id, ct);
//        }

//        public async Task AddAsync(Truck truck, CancellationToken ct = default)
//        {
//            await _context.Trucks.AddAsync(truck, ct);
//        }

//        public void Update(Truck truck)
//        {
//            _context.Trucks.Update(truck);
//        }

//        public void Remove(Truck truck)
//        {
//            _context.Trucks.Remove(truck);
//        }

//        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
//        {
//            return _context.Trucks.AnyAsync(x => x.Id == id, ct);
//        }
//    }
//}
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class TruckRepository : ITruckRepository
    {
        private readonly ApplicationContext _context;

        public TruckRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<Truck>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.Trucks
                .Include(x => x.TruckType)
                .Include(x => x.Driver)
                    .ThenInclude(d => d.Area)
                .ToListAsync(ct);
        }

        public Task<Truck?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.Trucks
                .Include(x => x.TruckType)
                .Include(x => x.Driver)
                    .ThenInclude(d => d.Area)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(Truck truck, CancellationToken ct = default)
        {
            await _context.Trucks.AddAsync(truck, ct);
        }

        public void Update(Truck truck)
        {
            _context.Trucks.Update(truck);
        }

        public void Remove(Truck truck)
        {
            _context.Trucks.Remove(truck);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.Trucks.AnyAsync(x => x.Id == id, ct);
        }
    }
}