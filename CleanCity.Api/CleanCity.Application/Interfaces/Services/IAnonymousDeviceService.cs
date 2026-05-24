using CleanCity.Application.DTOs.Devices;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IAnonymousDeviceService
    {
        Task<List<AnonymousDevice>> GetAllAsync(CancellationToken ct);
        Task<AnonymousDevice?> GetByIdAsync(int id, CancellationToken ct);

        Task<int> CreateAsync(AnonymousDevice device, CancellationToken ct);
        Task UpdateAsync(AnonymousDevice device, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);

        Task<AnonymousDevice?> GetByDeviceIdentifierAsync(string deviceIdentifier, CancellationToken ct);

        Task<RegisterAnonymousDeviceResultDto> RegisterOrUpdateAsync(
            RegisterAnonymousDeviceDto dto,
            CancellationToken ct);
    }
}