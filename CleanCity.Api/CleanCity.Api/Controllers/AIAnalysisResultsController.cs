using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisResultsController : ControllerBase
    {
        private readonly IAIAnalysisResultService _results;

        public AIAnalysisResultsController(IAIAnalysisResultService results)
        {
            _results = results;
        }

        [HttpGet]
        public async Task<ActionResult<List<AIAnalysisResult>>> GetAll(CancellationToken ct)
        {
            var items = await _results.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AIAnalysisResult>> GetById(int id, CancellationToken ct)
        {
            var item = await _results.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("by-report/{reportId:int}")]
        public async Task<ActionResult<AIAnalysisResult>> GetByReportId(int reportId, CancellationToken ct)
        {
            var item = await _results.GetByReportIdAsync(reportId, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AIAnalysisResult result, CancellationToken ct)
        {
            var id = await _results.CreateAsync(result, ct);
            return CreatedAtAction(nameof(GetById), new { id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] AIAnalysisResult result, CancellationToken ct)
        {
            if (id != result.Id) return BadRequest("Id mismatch.");

            try
            {
                await _results.UpdateAsync(result, ct);
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
                await _results.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
