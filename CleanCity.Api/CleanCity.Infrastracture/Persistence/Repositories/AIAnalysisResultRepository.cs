using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class AIAnalysisResultRepository : IAIAnalysisResultRepository
    {
        private readonly ApplicationContext _context;

        public AIAnalysisResultRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<AIAnalysisResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.AIAnalysisResults
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<AIAnalysisResult?> GetByReportIdAsync(int reportId, CancellationToken ct = default)
        {
            return _context.AIAnalysisResults
                .FirstOrDefaultAsync(x => x.ReportId == reportId, ct);
        }

        public Task<List<AIAnalysisResult>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.AIAnalysisResults
                .Include(x => x.Report)
                .ToListAsync(ct);
        }

        public async Task AddAsync(AIAnalysisResult result, CancellationToken ct = default)
        {
            await _context.AIAnalysisResults.AddAsync(result, ct);
        }

        public void Update(AIAnalysisResult result)
        {
            _context.AIAnalysisResults.Update(result);
        }

        public void Remove(AIAnalysisResult result)
        {
            _context.AIAnalysisResults.Remove(result);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.AIAnalysisResults
                .AnyAsync(x => x.Id == id, ct);
        }
    }
}
