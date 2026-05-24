using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;

namespace CleanCity.Application.Services
{
    public class AIAnalysisResultService : IAIAnalysisResultService
    {
        private readonly IAIAnalysisResultRepository _results;
        private readonly IReportRepository _reports;
        private readonly IUnitOfWork _uow;

        public AIAnalysisResultService(
            IAIAnalysisResultRepository results,
            IReportRepository reports,
            IUnitOfWork uow)
        {
            _results = results;
            _reports = reports;
            _uow = uow;
        }

        public async Task<int> CreateAsync(AIAnalysisResult result, CancellationToken ct = default)
        {
            var report = await _reports.GetByIdAsync(result.ReportId, ct);
            if (report == null)
                throw new InvalidOperationException("البلاغ غير موجود.");

            await _results.AddAsync(result, ct);
            await _uow.SaveChangesAsync(ct);
            return result.Id;
        }

        public async Task UpdateAsync(AIAnalysisResult result, CancellationToken ct = default)
        {
            var db = await _results.GetByIdAsync(result.Id, ct);
            if (db == null)
                throw new InvalidOperationException("نتيجة التحليل غير موجودة.");

            db.IsGarbageDetected = result.IsGarbageDetected;
            db.ConfidencePercentage = result.ConfidencePercentage;
            db.RejectionReason = result.RejectionReason;
            db.AnalyzedAt = result.AnalyzedAt;

            _results.Update(db);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var db = await _results.GetByIdAsync(id, ct);
            if (db == null)
                throw new InvalidOperationException("نتيجة التحليل غير موجودة.");

            _results.Remove(db);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<AIAnalysisResult?> GetByIdAsync(int id, CancellationToken ct = default)
            => _results.GetByIdAsync(id, ct);

        public Task<List<AIAnalysisResult>> GetAllAsync(CancellationToken ct = default)
            => _results.GetAllAsync(ct);
        public Task<AIAnalysisResult?> GetByReportIdAsync(int reportId, CancellationToken ct = default)
            => _results.GetByReportIdAsync(reportId, ct);

    }
}
