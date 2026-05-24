using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface ISewageServiceProviderRepository
    {
        // جلب كل الشركات
        Task<List<SewageServiceProvider>> GetAllAsync(CancellationToken ct = default);

        // جلب شركة واحدة بالمعرف
        Task<SewageServiceProvider?> GetByIdAsync(int id, CancellationToken ct = default);

        // إضافة شركة جديدة
        Task AddAsync(SewageServiceProvider item, CancellationToken ct = default);

        // تعديل
        void Update(SewageServiceProvider item);

        // حذف
        void Remove(SewageServiceProvider item);

        // هل موجود؟
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
