using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notifications;
        private readonly INotificationRecipientRepository _recipients;
        private readonly IUnitOfWork _uow;

        public NotificationService(
            INotificationRepository notifications,
            INotificationRecipientRepository recipients,
            IUnitOfWork uow)
        {
            _notifications = notifications;
            _recipients = recipients;
            _uow = uow;
        }

        public async Task<int> CreateAsync(
            Notification notification,
            List<NotificationRecipient> recipients,
            CancellationToken ct = default)
        {
            // 1) إضافة الإشعار نفسه
            await _notifications.AddAsync(notification, ct);

            // 2) ربط المستلمين بالإشعار
            foreach (var r in recipients)
            {
                r.Notification = notification;
                // لو NotificationId يتعبّى من EF بعد الحفظ ممكن يكفي ربط الـ Navigation
                await _recipients.AddAsync(r, ct);
            }

            // 3) حفظ في قاعدة البيانات
            await _uow.SaveChangesAsync(ct);

            return notification.Id;
        }

        public Task<List<NotificationRecipient>> GetForDriverAsync(
            int driverId,
            CancellationToken ct = default)
        {
            return _recipients.GetByDriverIdAsync(driverId, ct);
        }

        public Task<List<NotificationRecipient>> GetForUserAsync(
            string userId,
            CancellationToken ct = default)
        {
            return _recipients.GetByUserIdAsync(userId, ct);
        }

        public Task<List<NotificationRecipient>> GetForAnonymousDeviceAsync(
            int anonymousDeviceId,
            CancellationToken ct = default)
        {
            return _recipients.GetByAnonymousDeviceIdAsync(anonymousDeviceId, ct);
        }

        public async Task MarkAsReadAsync(int recipientId, CancellationToken ct = default)
        {
            var recipient = await _recipients.GetByIdAsync(recipientId, ct);
            if (recipient == null)
                throw new InvalidOperationException("Notification recipient not found.");

            if (!recipient.IsRead)
            {
                recipient.IsRead = true;
                recipient.ReadAt = DateTime.UtcNow;

                _recipients.Update(recipient);
                await _uow.SaveChangesAsync(ct);
            }
        }
    }
}
