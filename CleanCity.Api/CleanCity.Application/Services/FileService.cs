using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Application.Interfaces.Storage;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _files;
        private readonly IUnitOfWork _uow;
         private readonly IFileStorage _storage;

        public FileService(
            IFileRepository files,
            IUnitOfWork uow,
            IFileStorage storage)
        {
            _files = files;
            _uow = uow;
            _storage = storage;
        }
        public async Task<(FileEntity file, StoredFileResult stored)> SaveReportImageAsync(
       int reportId,
       Stream fileStream,
       string originalFileName,
       string contentType,
       CancellationToken ct = default)
        {
            var stored = await _storage.SaveAsync(fileStream, originalFileName, contentType, ct);

            var entity = new FileEntity
            {
                ReportId = reportId,
                FileName = stored.OriginalFileName,
                FilePath = stored.RelativePath,
                CreatedAt = DateTime.UtcNow
            };

            await _files.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return (entity, stored);
        }

        public async Task<int> CreateAsync(FileEntity file, CancellationToken ct = default)
        {
            file.CreatedAt = DateTime.UtcNow;

            await _files.AddAsync(file, ct);
            await _uow.SaveChangesAsync(ct);
            return file.Id;
        }

        public async Task UpdateAsync(FileEntity file, CancellationToken ct = default)
        {
            var db = await _files.GetByIdAsync(file.Id, ct);
            if (db == null)
                throw new InvalidOperationException("الملف غير موجود.");

            db.FileName = file.FileName;
            db.FilePath = file.FilePath;

            _files.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _files.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("الملف غير موجود.");

            _files.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<FileEntity?> GetByIdAsync(int id, CancellationToken ct = default)
            => _files.GetByIdAsync(id, ct);

        public Task<List<FileEntity>> GetAllAsync(CancellationToken ct = default)
            => _files.GetAllAsync(ct);
        public async Task<List<FileEntity>> GetByReportIdAsync(int reportId, CancellationToken ct = default)
        {
            return await _files.GetByReportIdAsync(reportId, ct);
        }

    }
}
