//using CleanCity.Application.Interfaces.Repositories;
//using CleanCity.Domain.Entities;
//using CleanCity.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;

//namespace CleanCity.Infrastructure.Persistence.Repositories
//{
//    public class DeviceTokenRepository : IDeviceTokenRepository
//    {
//        private readonly ApplicationContext _context;

//        public DeviceTokenRepository(ApplicationContext context)
//        {
//            _context = context;
//        }

//        // ============ CRUD ============

//        public Task<DeviceToken?> GetByIdAsync(int id, CancellationToken ct = default)
//        {
//            return _context.DeviceTokens
//                .FirstOrDefaultAsync(x => x.Id == id, ct);
//        }

//        public Task<List<DeviceToken>> GetAllAsync(CancellationToken ct = default)
//        {
//            return _context.DeviceTokens
//                .ToListAsync(ct);
//        }

//        public async Task AddAsync(DeviceToken token, CancellationToken ct = default)
//        {
//            await _context.DeviceTokens.AddAsync(token, ct);
//        }

//        public void Update(DeviceToken token)
//        {
//            _context.DeviceTokens.Update(token);
//        }

//        public void Remove(DeviceToken token)
//        {
//            _context.DeviceTokens.Remove(token);
//        }

//        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
//        {
//            return _context.DeviceTokens
//                .AnyAsync(x => x.Id == id, ct);
//        }

//        // ============ دوال خاصة ============

//        public Task<List<DeviceToken>> GetActiveTokensByUserIdAsync(string userId, CancellationToken ct = default)
//        {
//            return _context.DeviceTokens
//                .Where(x => x.UserId == userId && x.IsActive)
//                .ToListAsync(ct);
//        }

//        public async Task DeactivateAsync(int tokenId, CancellationToken ct = default)
//        {
//            var token = await _context.DeviceTokens
//                .FirstOrDefaultAsync(x => x.Id == tokenId, ct);

//            if (token == null)
//                return; // أو ترمي استثناء لو حاب

//            token.IsActive = false;
//            _context.DeviceTokens.Update(token);
//            // لا يوجد SaveChanges هنا، لأنه مسؤولية الـ UnitOfWork
//        }
//    }
//}
