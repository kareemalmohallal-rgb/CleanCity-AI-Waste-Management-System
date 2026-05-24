using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwarenessContentsController : ControllerBase
    {
        private readonly IAwarenessContentService _contents;

        public AwarenessContentsController(IAwarenessContentService contents)
        {
            _contents = contents;
        }

        [HttpGet]
        public async Task<ActionResult<List<AwarenessContent>>> GetAll(CancellationToken ct)
        {
            var items = await _contents.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AwarenessContent>> GetById(int id, CancellationToken ct)
        {
            var item = await _contents.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AwarenessContent content, CancellationToken ct)
        {
            var id = await _contents.CreateAsync(content, ct);
            return CreatedAtAction(nameof(GetById), new { id }, content);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] AwarenessContent content, CancellationToken ct)
        {
            if (id != content.Id) return BadRequest("Id mismatch.");

            try
            {
                await _contents.UpdateAsync(content, ct);
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
                await _contents.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
