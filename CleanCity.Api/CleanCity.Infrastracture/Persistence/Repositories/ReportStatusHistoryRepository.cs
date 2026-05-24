using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class ReportStatusHistoryRepository : IReportStatusHistoryRepository
    {
        private readonly ApplicationContext _context;

        public ReportStatusHistoryRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReportStatusHistory history, CancellationToken ct = default)
        {
            await _context.ReportStatusHistories.AddAsync(history, ct);
        }

        public Task<ReportStatusHistory?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.ReportStatusHistories
                .Include(x => x.Report)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<List<ReportStatusHistory>> GetByReportIdAsync(int reportId, CancellationToken ct = default)
        {
            return _context.ReportStatusHistories
                .Where(x => x.ReportId == reportId)
                .OrderBy(x => x.UpdatedAt)
                .ToListAsync(ct);
        }

        public Task<List<ReportStatusHistory>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.ReportStatusHistories
                .Include(x => x.Report)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync(ct);
        }

        public void Remove(ReportStatusHistory history)
        {
            _context.ReportStatusHistories.Remove(history);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.ReportStatusHistories
                .AnyAsync(x => x.Id == id, ct);
        }
    }
}
