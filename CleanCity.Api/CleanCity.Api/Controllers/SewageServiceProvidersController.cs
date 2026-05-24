using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SewageServiceProvidersController : ControllerBase
    {
        private readonly ISewageServiceProviderService _providers;

        public SewageServiceProvidersController(ISewageServiceProviderService providers)
        {
            _providers = providers;
        }

        [HttpGet]
        public async Task<ActionResult<List<SewageServiceProvider>>> GetAll(CancellationToken ct)
        {
            var items = await _providers.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SewageServiceProvider>> GetById(int id, CancellationToken ct)
        {
            var item = await _providers.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] SewageServiceProvider provider, CancellationToken ct)
        {
            var id = await _providers.CreateAsync(provider, ct);
            return CreatedAtAction(nameof(GetById), new { id }, provider);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] SewageServiceProvider provider, CancellationToken ct)
        {
            if (id != provider.Id) return BadRequest("Id mismatch.");

            try
            {
                await _providers.UpdateAsync(provider, ct);
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
                await _providers.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
