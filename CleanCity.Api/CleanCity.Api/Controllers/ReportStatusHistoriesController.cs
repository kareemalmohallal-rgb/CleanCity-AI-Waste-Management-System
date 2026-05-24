using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportStatusHistoriesController : ControllerBase
    {
        private readonly IReportStatusHistoryService _history;

        public ReportStatusHistoriesController(IReportStatusHistoryService history)
        {
            _history = history;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReportStatusHistory>> GetById(int id, CancellationToken ct)
        {
            var item = await _history.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("by-report/{reportId:int}")]
        public async Task<ActionResult<List<ReportStatusHistory>>> GetByReportId(int reportId, CancellationToken ct)
        {
            var items = await _history.GetByReportIdAsync(reportId, ct);
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ReportStatusHistory history, CancellationToken ct)
        {
            var id = await _history.CreateAsync(history, ct);
            return CreatedAtAction(nameof(GetById), new { id }, history);
        }
    }
}
