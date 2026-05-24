using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IFileRepository
    {
        // جلب ملف بالمعرّف
        Task<FileEntity?> GetByIdAsync(int id, CancellationToken ct = default);

        // جلب كل الملفات
        Task<List<FileEntity>> GetAllAsync(CancellationToken ct = default);

        // إضافة ملف جديد
        Task AddAsync(FileEntity file, CancellationToken ct = default);

        // تعديل ملف
        void Update(FileEntity file);

        // حذف ملف
        void Remove(FileEntity file);

        // جلب الملفات حسب البلاغ
        Task<List<FileEntity>> GetByReportIdAsync(int reportId, CancellationToken ct = default);

        // التحقق من وجود ملف
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
