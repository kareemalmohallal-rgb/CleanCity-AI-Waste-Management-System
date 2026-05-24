using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface INotificationService
    {
        // إنشاء إشعار مع المستلمين
        Task<int> CreateAsync(
            Notification notification,
            List<NotificationRecipient> recipients,
            CancellationToken ct = default);

        // جلب الإشعارات لسائق معيّن
        Task<List<NotificationRecipient>> GetForDriverAsync(
            int driverId,
            CancellationToken ct = default);

        // جلب الإشعارات لمستخدم (Identity User) معيّن
        Task<List<NotificationRecipient>> GetForUserAsync(
            string userId,
            CancellationToken ct = default);

        // جلب الإشعارات لجهاز مجهول (AnonymousDevice)
        Task<List<NotificationRecipient>> GetForAnonymousDeviceAsync(
            int anonymousDeviceId,
            CancellationToken ct = default);

        // تعليم إشعار واحد كمقروء
        Task MarkAsReadAsync(int recipientId, CancellationToken ct = default);
    }
}
