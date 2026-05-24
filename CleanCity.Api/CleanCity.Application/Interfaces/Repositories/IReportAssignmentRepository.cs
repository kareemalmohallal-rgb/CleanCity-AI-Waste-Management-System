using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IReportAssignmentRepository
    {
        // إضافة إسناد
        Task AddAsync(ReportAssignment assignment, CancellationToken ct = default);

        // تعديل
        void Update(ReportAssignment assignment);

        // حذف
        void Remove(ReportAssignment assignment);

        // جلب واحد بالمعرّف
        Task<ReportAssignment?> GetByIdAsync(int id, CancellationToken ct = default);

        // جلب كل الإسنادات
        Task<List<ReportAssignment>> GetAllAsync(CancellationToken ct = default);

        // جلب جميع إسنادات بلاغ معيّن
        Task<List<ReportAssignment>> GetByReportIdAsync(int reportId, CancellationToken ct = default);

        // التحقق من الوجود
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<ReportAssignment?> GetCurrentAssignedAsync(
    int reportId,
    int driverId,
    CancellationToken ct = default);
    }
}
