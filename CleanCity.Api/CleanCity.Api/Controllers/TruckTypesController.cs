using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TruckTypesController : ControllerBase
    {
        private readonly ITruckTypeService _types;

        public TruckTypesController(ITruckTypeService types)
        {
            _types = types;
        }

        // GET: api/trucktypes
        [HttpGet]
        public async Task<ActionResult<List<TruckType>>> GetAll(CancellationToken ct)
        {
            var items = await _types.GetAllAsync(ct);
            return Ok(items);
        }

        // GET: api/trucktypes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TruckType>> GetById(int id, CancellationToken ct)
        {
            var item = await _types.GetByIdAsync(id, ct);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/trucktypes
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TruckType type, CancellationToken ct)
        {
            var id = await _types.CreateAsync(type, ct);
            return CreatedAtAction(nameof(GetById), new { id }, type);
        }

        // PUT: api/trucktypes/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] TruckType type, CancellationToken ct)
        {
            if (id != type.Id)
                return BadRequest("TruckType id mismatch.");

            try
            {
                await _types.UpdateAsync(type, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/trucktypes/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _types.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
