using CleanCity.Application.DTOs.Devices;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnonymousDevicesController : ControllerBase
    {
        private readonly IAnonymousDeviceService _devices;

        public AnonymousDevicesController(IAnonymousDeviceService devices)
        {
            _devices = devices;
        }

        [HttpGet]
        public async Task<ActionResult<List<AnonymousDevice>>> GetAll(CancellationToken ct)
        {
            var items = await _devices.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AnonymousDevice>> GetById(int id, CancellationToken ct)
        {
            var item = await _devices.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("by-device-identifier/{deviceIdentifier}")]
        public async Task<ActionResult<AnonymousDevice>> GetByDeviceIdentifier(
            string deviceIdentifier,
            CancellationToken ct)
        {
            var item = await _devices.GetByDeviceIdentifierAsync(deviceIdentifier, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AnonymousDevice device, CancellationToken ct)
        {
            var id = await _devices.CreateAsync(device, ct);
            return CreatedAtAction(nameof(GetById), new { id }, device);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterAnonymousDeviceResultDto>> Register(
            [FromBody] RegisterAnonymousDeviceDto dto,
            CancellationToken ct)
        {
            var result = await _devices.RegisterOrUpdateAsync(dto, ct);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] AnonymousDevice device, CancellationToken ct)
        {
            if (id != device.Id) return BadRequest("Id mismatch.");

            try
            {
                await _devices.UpdateAsync(device, ct);
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
                await _devices.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}