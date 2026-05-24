using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class SewageServiceProviderService : ISewageServiceProviderService
    {
        private readonly ISewageServiceProviderRepository _providers;
        private readonly IUnitOfWork _uow;

        public SewageServiceProviderService(
            ISewageServiceProviderRepository providers,
            IUnitOfWork uow)
        {
            _providers = providers;
            _uow = uow;
        }

        public async Task<int> CreateAsync(SewageServiceProvider provider, CancellationToken ct = default)
        {
            await _providers.AddAsync(provider, ct);
            await _uow.SaveChangesAsync(ct);
            return provider.Id;
        }

        public async Task UpdateAsync(SewageServiceProvider provider, CancellationToken ct = default)
        {
            var db = await _providers.GetByIdAsync(provider.Id, ct);
            if (db == null)
                throw new InvalidOperationException("المزود غير موجود.");

            db.OwnerName = provider.OwnerName;
            db.PhoneNumber = provider.PhoneNumber;
            db.Size = provider.Size;
            db.Price = provider.Price;

            _providers.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _providers.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("المزود غير موجود.");

            _providers.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<SewageServiceProvider?> GetByIdAsync(int id, CancellationToken ct = default)
            => _providers.GetByIdAsync(id, ct);

        public Task<List<SewageServiceProvider>> GetAllAsync(CancellationToken ct = default)
            => _providers.GetAllAsync(ct);
    }
}
