using CleanCity.Application.Interfaces.Storage;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<int> CreateAsync(FileEntity file, CancellationToken ct = default);
        Task UpdateAsync(FileEntity file, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<FileEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<FileEntity>> GetAllAsync(CancellationToken ct = default);
        Task<List<FileEntity>> GetByReportIdAsync(int reportId, CancellationToken ct = default);
        // ✅ جديد: حفظ صورة بلاغ
        Task<(FileEntity file, StoredFileResult stored)> SaveReportImageAsync(
            int reportId,
            Stream fileStream,
            string originalFileName,
            string contentType,
            CancellationToken ct = default);


    }
}
