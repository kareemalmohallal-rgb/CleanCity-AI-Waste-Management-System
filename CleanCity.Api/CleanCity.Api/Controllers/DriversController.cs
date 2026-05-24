//using CleanCity.Application.Interfaces.Services;
//using CleanCity.Domain.Entities;
//using Microsoft.AspNetCore.Mvc;

//namespace CleanCity.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class DriversController : ControllerBase
//    {
//        private readonly IDriverService _drivers;

//        public DriversController(IDriverService drivers)
//        {
//            _drivers = drivers;
//        }

//        // GET: api/drivers
//        [HttpGet]
//        public async Task<ActionResult<List<Driver>>> GetAll(CancellationToken ct)
//        {
//            var items = await _drivers.GetAllAsync(ct);
//            return Ok(items);
//        }

//        // GET: api/drivers/5
//        [HttpGet("{id:int}")]
//        public async Task<ActionResult<Driver>> GetById(int id, CancellationToken ct)
//        {
//            var driver = await _drivers.GetByIdAsync(id, ct);
//            if (driver == null)
//                return NotFound();

//            return Ok(driver);
//        }

//        // POST: api/drivers
//        [HttpPost]
//        public async Task<ActionResult> Create([FromBody] Driver driver, CancellationToken ct)
//        {
//            var id = await _drivers.CreateAsync(driver, ct);
//            return CreatedAtAction(nameof(GetById), new { id }, driver);
//        }

//        // PUT: api/drivers/5
//        [HttpPut("{id:int}")]
//        public async Task<ActionResult> Update(int id, [FromBody] Driver driver, CancellationToken ct)
//        {
//            if (id != driver.Id)
//                return BadRequest("Driver id mismatch.");

//            try
//            {
//                await _drivers.UpdateAsync(driver, ct);
//                return NoContent();
//            }
//            catch (InvalidOperationException ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }

//        // DELETE: api/drivers/5
//        [HttpDelete("{id:int}")]
//        public async Task<ActionResult> Delete(int id, CancellationToken ct)
//        {
//            try
//            {
//                await _drivers.DeleteAsync(id, ct);
//                return NoContent();
//            }
//            catch (InvalidOperationException ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }
//    }
//}
using CleanCity.Application.DTOs.Drivers;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _drivers;

        public DriversController(IDriverService drivers)
        {
            _drivers = drivers;
        }

        // GET: api/drivers
        [HttpGet]
        public async Task<ActionResult<List<DriverReadDto>>> GetAll(CancellationToken ct)
        {
            var items = await _drivers.GetAllAsync(ct);
            var dtos = items.Select(ToDto).ToList();
            return Ok(dtos);
        }

        // GET: api/drivers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DriverReadDto>> GetById(int id, CancellationToken ct)
        {
            var driver = await _drivers.GetByIdAsync(id, ct);
            if (driver == null)
                return NotFound();

            return Ok(ToDto(driver));
        }

        // POST: api/drivers
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Driver driver, CancellationToken ct)
        {
            var id = await _drivers.CreateAsync(driver, ct);
            return CreatedAtAction(nameof(GetById), new { id }, ToDto(driver));
        }

        // PUT: api/drivers/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Driver driver, CancellationToken ct)
        {
            if (id != driver.Id)
                return BadRequest("Driver id mismatch.");

            try
            {
                await _drivers.UpdateAsync(driver, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/drivers/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _drivers.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // ✅ تحويل داخلي من Entity إلى DTO — يقطع الدورة
        private static DriverReadDto ToDto(Driver d) => new()
        {
            Id = d.Id,
            FullName = d.FullName,
            PhoneNumber = d.PhoneNumber,
            LicenseNumber = d.LicenseNumber,
            IsActive = d.IsActive,
            AreaId = d.AreaId,
        };
    }
}