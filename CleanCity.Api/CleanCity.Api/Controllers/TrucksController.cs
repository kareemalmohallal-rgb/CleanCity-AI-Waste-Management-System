using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrucksController : ControllerBase
    {
        private readonly ITruckService _trucks;

        public TrucksController(ITruckService trucks)
        {
            _trucks = trucks;
        }

        // GET: api/trucks
        [HttpGet]
        public async Task<ActionResult<List<Truck>>> GetAll(CancellationToken ct)
        {
            var items = await _trucks.GetAllAsync(ct);
            return Ok(items);
        }

        // GET: api/trucks/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Truck>> GetById(int id, CancellationToken ct)
        {
            var item = await _trucks.GetByIdAsync(id, ct);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/trucks
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Truck truck, CancellationToken ct)
        {
            var id = await _trucks.CreateAsync(truck, ct);
            return CreatedAtAction(nameof(GetById), new { id }, truck);
        }

        // PUT: api/trucks/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Truck truck, CancellationToken ct)
        {
            if (id != truck.Id)
                return BadRequest("Truck id mismatch.");

            try
            {
                await _trucks.UpdateAsync(truck, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/trucks/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _trucks.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
