using CleanCity.Domain.Entities;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IAIAnalysisResultService
    {
        Task<int> CreateAsync(AIAnalysisResult result, CancellationToken ct = default);
        Task UpdateAsync(AIAnalysisResult result, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<AIAnalysisResult?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<AIAnalysisResult>> GetAllAsync(CancellationToken ct = default);
        Task<AIAnalysisResult?> GetByReportIdAsync(int reportId, CancellationToken ct = default);

    }
}
