using CleanCity.Application.DTOs.Devices;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class AnonymousDeviceService : IAnonymousDeviceService
    {
        private readonly IAnonymousDeviceRepository _repo;

        public AnonymousDeviceService(IAnonymousDeviceRepository repo)
        {
            _repo = repo;
        }

        public Task<List<AnonymousDevice>> GetAllAsync(CancellationToken ct)
        {
            return _repo.GetAllAsync(ct);
        }

        public Task<AnonymousDevice?> GetByIdAsync(int id, CancellationToken ct)
        {
            return _repo.GetByIdAsync(id, ct);
        }

        public Task<AnonymousDevice?> GetByDeviceIdentifierAsync(
            string deviceIdentifier,
            CancellationToken ct)
        {
            return _repo.GetByDeviceIdentifierAsync(deviceIdentifier, ct);
        }

        public async Task<int> CreateAsync(AnonymousDevice device, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(device.DeviceIdentifier))
                throw new ArgumentException("DeviceIdentifier is required.");

            if (string.IsNullOrWhiteSpace(device.FcmToken))
                throw new ArgumentException("FcmToken is required.");

            device.CreatedAt = DateTime.UtcNow;

            return await _repo.CreateAsync(device, ct);
        }

        public async Task UpdateAsync(AnonymousDevice device, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(device.Id, ct);
            if (existing == null)
                throw new InvalidOperationException("AnonymousDevice not found.");

            existing.DeviceIdentifier = device.DeviceIdentifier;
            existing.FcmToken = device.FcmToken;

            await _repo.UpdateAsync(existing, ct);
        }

        public Task DeleteAsync(int id, CancellationToken ct)
        {
            return _repo.DeleteAsync(id, ct);
        }

        public async Task<RegisterAnonymousDeviceResultDto> RegisterOrUpdateAsync(
            RegisterAnonymousDeviceDto dto,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.DeviceIdentifier))
                throw new ArgumentException("DeviceIdentifier is required.");

            if (string.IsNullOrWhiteSpace(dto.FcmToken))
                throw new ArgumentException("FcmToken is required.");

            var existing = await _repo.GetByDeviceIdentifierAsync(dto.DeviceIdentifier, ct);

            if (existing != null)
            {
                existing.FcmToken = dto.FcmToken;
                await _repo.UpdateAsync(existing, ct);

                return new RegisterAnonymousDeviceResultDto
                {
                    Id = existing.Id,
                    DeviceIdentifier = existing.DeviceIdentifier,
                    FcmToken = existing.FcmToken
                };
            }

            var entity = new AnonymousDevice
            {
                DeviceIdentifier = dto.DeviceIdentifier,
                FcmToken = dto.FcmToken,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _repo.CreateAsync(entity, ct);

            return new RegisterAnonymousDeviceResultDto
            {
                Id = id,
                DeviceIdentifier = entity.DeviceIdentifier,
                FcmToken = entity.FcmToken
            };
        }
    }
}