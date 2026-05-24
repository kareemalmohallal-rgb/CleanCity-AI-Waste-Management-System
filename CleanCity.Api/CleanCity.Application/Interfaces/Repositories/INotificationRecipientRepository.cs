using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface INotificationRecipientRepository
    {
        Task AddAsync(NotificationRecipient recipient, CancellationToken ct = default);

        void Update(NotificationRecipient recipient);

        Task<NotificationRecipient?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<List<NotificationRecipient>> GetByDriverIdAsync(int driverId, CancellationToken ct = default);

        Task<List<NotificationRecipient>> GetByUserIdAsync(string userId, CancellationToken ct = default);

        Task<List<NotificationRecipient>> GetByAnonymousDeviceIdAsync(int anonymousDeviceId, CancellationToken ct = default);
    }
}
