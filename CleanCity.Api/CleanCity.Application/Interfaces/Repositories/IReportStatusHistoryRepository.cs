using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IReportStatusHistoryRepository
    {
        // إضافة سجل جديد في تاريخ الحالة
        Task AddAsync(ReportStatusHistory history, CancellationToken ct = default);

        // جلب سجل واحد بالمعرّف
        Task<ReportStatusHistory?> GetByIdAsync(int id, CancellationToken ct = default);

        // جلب كل السجلات لبلاغ معيّن
        Task<List<ReportStatusHistory>> GetByReportIdAsync(int reportId, CancellationToken ct = default);

        // جلب جميع السجلات (لو احتجتها لتقارير عامة)
        Task<List<ReportStatusHistory>> GetAllAsync(CancellationToken ct = default);

        // حذف سجل (اختياري لو حاب تمسح سجلات قديمة)
        void Remove(ReportStatusHistory history);

        // التحقق من وجود سجل
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
