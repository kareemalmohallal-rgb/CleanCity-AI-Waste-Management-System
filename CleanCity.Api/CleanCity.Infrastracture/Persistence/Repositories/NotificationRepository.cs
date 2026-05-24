using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationContext _context;

        public NotificationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
            => await _context.Notifications.AddAsync(notification, ct);
    }
}
