//using CleanCity.Application.Interfaces;
//using CleanCity.Application.Interfaces.Repositories;
//using CleanCity.Application.Interfaces.Services;
//using CleanCity.Domain.Entities;

//namespace CleanCity.Application.Services
//{
//    public class DeviceTokenService : IDeviceTokenService
//    {
//        private readonly IDeviceTokenRepository _tokens;
//        private readonly IUnitOfWork _uow;

//        public DeviceTokenService(
//            IDeviceTokenRepository tokens,
//            IUnitOfWork uow)
//        {
//            _tokens = tokens;
//            _uow = uow;
//        }

//        public async Task<int> CreateAsync(DeviceToken token, CancellationToken ct = default)
//        {
//            await _tokens.AddAsync(token, ct);
//            await _uow.SaveChangesAsync(ct);
//            return token.Id;
//        }

//        public async Task UpdateAsync(DeviceToken token, CancellationToken ct = default)
//        {
//            var db = await _tokens.GetByIdAsync(token.Id, ct);
//            if (db == null)
//                throw new InvalidOperationException("DeviceToken غير موجود.");

//            db.UserId = token.UserId;
//            db.Token = token.Token;
//            db.DeviceType = token.DeviceType;
//            db.IsActive = token.IsActive;

//            _tokens.Update(db);
//            await _uow.SaveChangesAsync(ct);
//        }

//        public async Task DeleteAsync(int id, CancellationToken ct = default)
//        {
//            var db = await _tokens.GetByIdAsync(id, ct);
//            if (db == null)
//                throw new InvalidOperationException("DeviceToken غير موجود.");

//            _tokens.Remove(db);
//            await _uow.SaveChangesAsync(ct);
//        }

//        public Task<DeviceToken?> GetByIdAsync(int id, CancellationToken ct = default)
//            => _tokens.GetByIdAsync(id, ct);

//        public Task<List<DeviceToken>> GetAllAsync(CancellationToken ct = default)
//            => _tokens.GetAllAsync(ct);
//    }
//}
