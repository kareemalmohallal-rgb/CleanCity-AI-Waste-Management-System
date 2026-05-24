using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class NotificationRecipientRepository : INotificationRecipientRepository
    {
        private readonly ApplicationContext _context;

        public NotificationRecipientRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationRecipient recipient, CancellationToken ct = default)
            => await _context.NotificationRecipients.AddAsync(recipient, ct);

        public void Update(NotificationRecipient recipient)
            => _context.NotificationRecipients.Update(recipient);

        public Task<NotificationRecipient?> GetByIdAsync(int id, CancellationToken ct = default)
            => _context.NotificationRecipients
                       .Include(x => x.Notification) // اختياري
                       .FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<List<NotificationRecipient>> GetByDriverIdAsync(int driverId, CancellationToken ct = default)
            => _context.NotificationRecipients
                       .Where(x => x.DriverId == driverId)
                       .Include(x => x.Notification)
                       .OrderByDescending(x => x.Notification.CreatedAt)
                       .ToListAsync(ct);

        public Task<List<NotificationRecipient>> GetByUserIdAsync(string userId, CancellationToken ct = default)
            => _context.NotificationRecipients
                       .Where(x => x.UserId == userId)
                       .Include(x => x.Notification)
                       .OrderByDescending(x => x.Notification.CreatedAt)
                       .ToListAsync(ct);

        public Task<List<NotificationRecipient>> GetByAnonymousDeviceIdAsync(int anonymousDeviceId, CancellationToken ct = default)
            => _context.NotificationRecipients
                       .Where(x => x.AnonymousDeviceId == anonymousDeviceId)
                       .Include(x => x.Notification)
                       .OrderByDescending(x => x.Notification.CreatedAt)
                       .ToListAsync(ct);
    }
}
