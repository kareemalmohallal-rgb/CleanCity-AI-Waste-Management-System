using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Domain.Entities;
using CleanCity.Domain.Enum;
using CleanCity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Infrastructure.Persistence.Repositories
{
    public class ReportAssignmentRepository : IReportAssignmentRepository
    {
        private readonly ApplicationContext _context;

        public ReportAssignmentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReportAssignment assignment, CancellationToken ct = default)
        {
            await _context.ReportAssignments.AddAsync(assignment, ct);
        }

        public void Update(ReportAssignment assignment)
        {
            _context.ReportAssignments.Update(assignment);
        }

        public void Remove(ReportAssignment assignment)
        {
            _context.ReportAssignments.Remove(assignment);
        }

        public Task<ReportAssignment?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _context.ReportAssignments
                .Include(x => x.Driver)
                .Include(x => x.Report)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<List<ReportAssignment>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.ReportAssignments
                .Include(x => x.Driver)
                .Include(x => x.Report)
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync(ct);
        }

        public Task<List<ReportAssignment>> GetByReportIdAsync(int reportId, CancellationToken ct = default)
        {
            return _context.ReportAssignments
                .Where(x => x.ReportId == reportId)
                .Include(x => x.Driver)
                .Include(x => x.Report)
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync(ct);
        }

        public Task<ReportAssignment?> GetCurrentAssignedAsync(
            int reportId,
            int driverId,
            CancellationToken ct = default)
        {
            return _context.ReportAssignments
                .Include(x => x.Driver)
                .Include(x => x.Report)
                .Where(x =>
                    x.ReportId == reportId &&
                    x.DriverId == driverId &&
                    x.Status == AssignmentStatus.Assigned)
                .OrderByDescending(x => x.AssignedAt)
                .FirstOrDefaultAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return _context.ReportAssignments.AnyAsync(x => x.Id == id, ct);
        }
    }
}