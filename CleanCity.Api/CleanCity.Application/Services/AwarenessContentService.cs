using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;

namespace CleanCity.Application.Services
{
    public class AwarenessContentService : IAwarenessContentService
    {
        private readonly IAwarenessContentRepository _contents;
        private readonly IAnonymousDeviceRepository _anonymousDevices;
        private readonly INotificationService _notificationService;
        private readonly IPushNotificationSender _pushSender;
        private readonly IUnitOfWork _uow;

        public AwarenessContentService(
            IAwarenessContentRepository contents,
            IAnonymousDeviceRepository anonymousDevices,
            INotificationService notificationService,
            IPushNotificationSender pushSender,
            IUnitOfWork uow)
        {
            _contents = contents;
            _anonymousDevices = anonymousDevices;
            _notificationService = notificationService;
            _pushSender = pushSender;
            _uow = uow;
        }

        public async Task<int> CreateAsync(AwarenessContent content, CancellationToken ct = default)
        {
            content.CreatedAt = DateTime.UtcNow;

            await _contents.AddAsync(content, ct);
            await _uow.SaveChangesAsync(ct);

            // 1) سجل الإشعار في قاعدة البيانات لكل الأجهزة المجهولة
            var devices = await _anonymousDevices.GetAllAsync(ct);

            if (devices.Count > 0)
            {
                var notification = new Notification
                {
                    Type = NotificationType.Awareness,
                    Title = "مقال توعوي جديد",
                    Body = content.Content,
                    DeepLink = $"/awareness/{content.Id}",
                    CreatedAt = DateTime.UtcNow,
                    AwarenessContentId = content.Id
                };

                var recipients = devices.Select(d => new NotificationRecipient
                {
                    AnonymousDeviceId = d.Id,
                    IsRead = false
                }).ToList();

                await _notificationService.CreateAsync(notification, recipients, ct);
            }

            // 2) أرسل Push Notification عام عبر Topic
            await _pushSender.SendToTopicAsync(
                topic: "awareness_all",
                title: "مقال توعوي جديد",
                body: content.Title,
                data: new Dictionary<string, string>
                {
                    ["type"] = "awareness",
                    ["awarenessId"] = content.Id.ToString()
                },
                ct: ct);

            return content.Id;
        }

        public async Task UpdateAsync(AwarenessContent content, CancellationToken ct = default)
        {
            var db = await _contents.GetByIdAsync(content.Id, ct);
            if (db == null)
                throw new InvalidOperationException("المحتوى غير موجود.");

            db.Title = content.Title;
            db.Content = content.Content;
            db.ImageUrl = content.ImageUrl;

            _contents.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _contents.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("المحتوى غير موجود.");

            _contents.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<AwarenessContent?> GetByIdAsync(int id, CancellationToken ct = default)
            => _contents.GetByIdAsync(id, ct);

        public Task<List<AwarenessContent>> GetAllAsync(CancellationToken ct = default)
            => _contents.GetAllAsync(ct);
    }
}