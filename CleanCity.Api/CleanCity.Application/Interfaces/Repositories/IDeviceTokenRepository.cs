//using CleanCity.Domain.Entities;

//namespace CleanCity.Application.Interfaces.Repositories
//{
//    public interface IDeviceTokenRepository
//    {
//        // CRUD أساسي
//        Task<DeviceToken?> GetByIdAsync(int id, CancellationToken ct = default);
//        Task<List<DeviceToken>> GetAllAsync(CancellationToken ct = default);
//        Task AddAsync(DeviceToken token, CancellationToken ct = default);
//        void Update(DeviceToken token);
//        void Remove(DeviceToken token);
//        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

//        // دوال خاصة بـ DeviceToken
//        Task<List<DeviceToken>> GetActiveTokensByUserIdAsync(string userId, CancellationToken ct = default);
//        Task DeactivateAsync(int tokenId, CancellationToken ct = default);
//    }
//}
