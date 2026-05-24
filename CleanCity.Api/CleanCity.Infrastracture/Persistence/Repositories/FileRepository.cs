using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationContext _context;

        public FileRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<FileEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.Files
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<List<FileEntity>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.Files
                .ToListAsync(ct);
        }

        public async Task AddAsync(FileEntity file, CancellationToken ct = default)
        {
            await _context.Files.AddAsync(file, ct);
        }

        public void Update(FileEntity file)
        {
            _context.Files.Update(file);
        }

        public void Remove(FileEntity file)
        {
            _context.Files.Remove(file);
        }

        public Task<List<FileEntity>> GetByReportIdAsync(int reportId, CancellationToken ct = default)
        {
            return _context.Files
                .Where(x => x.ReportId == reportId)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.Files
                .AnyAsync(x => x.Id == id, ct);
        }
    }
}
