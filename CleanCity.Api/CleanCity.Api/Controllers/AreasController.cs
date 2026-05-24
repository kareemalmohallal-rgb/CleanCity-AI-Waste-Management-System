using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _areas;

        public AreasController(IAreaService areas)
        {
            _areas = areas;
        }

        [HttpGet]
        public async Task<ActionResult<List<Area>>> GetAll(CancellationToken ct)
        {
            var items = await _areas.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Area>> GetById(int id, CancellationToken ct)
        {
            var item = await _areas.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Area area, CancellationToken ct)
        {
            var id = await _areas.CreateAsync(area, ct);
            return CreatedAtAction(nameof(GetById), new { id }, area);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Area area, CancellationToken ct)
        {
            if (id != area.Id) return BadRequest("Id mismatch");

            await _areas.UpdateAsync(area, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            await _areas.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
