using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IAwarenessContentRepository
    {
        // جلب كل المحتوى
        Task<List<AwarenessContent>> GetAllAsync(CancellationToken ct = default);

        // جلب عنصر واحد
        Task<AwarenessContent?> GetByIdAsync(int id, CancellationToken ct = default);

        // إضافة محتوى جديد
        Task AddAsync(AwarenessContent item, CancellationToken ct = default);

        // تعديل المحتوى
        void Update(AwarenessContent item);

        // حذف المحتوى
        void Remove(AwarenessContent item);

        // التأكد من وجود العنصر
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
