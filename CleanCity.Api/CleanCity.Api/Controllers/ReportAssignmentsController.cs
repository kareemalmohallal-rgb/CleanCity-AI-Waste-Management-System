using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportAssignmentsController : ControllerBase
    {
        private readonly IReportAssignmentService _assignments;

        public ReportAssignmentsController(IReportAssignmentService assignments)
        {
            _assignments = assignments;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReportAssignment>>> GetAll(CancellationToken ct)
        {
            var items = await _assignments.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReportAssignment>> GetById(int id, CancellationToken ct)
        {
            var item = await _assignments.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("by-report/{reportId:int}")]
        public async Task<ActionResult<List<ReportAssignment>>> GetByReportId(int reportId, CancellationToken ct)
        {
            var items = await _assignments.GetByReportIdAsync(reportId, ct);
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ReportAssignment assignment, CancellationToken ct)
        {
            var id = await _assignments.CreateAsync(assignment, ct);
            return CreatedAtAction(nameof(GetById), new { id }, assignment);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] ReportAssignment assignment, CancellationToken ct)
        {
            if (id != assignment.Id) return BadRequest("Id mismatch.");

            try
            {
                await _assignments.UpdateAsync(assignment, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _assignments.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
