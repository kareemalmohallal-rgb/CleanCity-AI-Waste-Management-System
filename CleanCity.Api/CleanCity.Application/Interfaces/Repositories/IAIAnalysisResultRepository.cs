using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IAIAnalysisResultRepository
    {
        // جلب بالمعرّف
        Task<AIAnalysisResult?> GetByIdAsync(int id, CancellationToken ct = default);

        // جلب حسب البلاغ
        Task<AIAnalysisResult?> GetByReportIdAsync(int reportId, CancellationToken ct = default);

        // جلب الكل
        Task<List<AIAnalysisResult>> GetAllAsync(CancellationToken ct = default);

        // إضافة
        Task AddAsync(AIAnalysisResult result, CancellationToken ct = default);

        // تعديل
        void Update(AIAnalysisResult result);

        // حذف
        void Remove(AIAnalysisResult result);

        // التحقق من الوجود
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
