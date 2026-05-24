using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken ct = default);
    }
}
