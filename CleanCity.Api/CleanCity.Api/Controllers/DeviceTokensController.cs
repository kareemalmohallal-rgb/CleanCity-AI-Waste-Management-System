//using CleanCity.Application.Interfaces.Services;
//using CleanCity.Domain.Entities;
//using Microsoft.AspNetCore.Mvc;

//namespace CleanCity.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class DeviceTokensController : ControllerBase
//    {
//        private readonly IDeviceTokenService _tokens;

//        public DeviceTokensController(IDeviceTokenService tokens)
//        {
//            _tokens = tokens;
//        }

//        [HttpGet]
//        public async Task<ActionResult<List<DeviceToken>>> GetAll(CancellationToken ct)
//        {
//            var items = await _tokens.GetAllAsync(ct);
//            return Ok(items);
//        }

//        [HttpGet("{id:int}")]
//        public async Task<ActionResult<DeviceToken>> GetById(int id, CancellationToken ct)
//        {
//            var item = await _tokens.GetByIdAsync(id, ct);
//            if (item == null) return NotFound();
//            return Ok(item);
//        }

//        [HttpPost]
//        public async Task<ActionResult> Create([FromBody] DeviceToken token, CancellationToken ct)
//        {
//            var id = await _tokens.CreateAsync(token, ct);
//            return CreatedAtAction(nameof(GetById), new { id }, token);
//        }

//        [HttpPut("{id:int}")]
//        public async Task<ActionResult> Update(int id, [FromBody] DeviceToken token, CancellationToken ct)
//        {
//            if (id != token.Id) return BadRequest("Id mismatch.");

//            try
//            {
//                await _tokens.UpdateAsync(token, ct);
//                return NoContent();
//            }
//            catch (InvalidOperationException ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }

//        [HttpDelete("{id:int}")]
//        public async Task<ActionResult> Delete(int id, CancellationToken ct)
//        {
//            try
//            {
//                await _tokens.DeleteAsync(id, ct);
//                return NoContent();
//            }
//            catch (InvalidOperationException ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }
//    }
//}
