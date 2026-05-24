using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileEntitiesController : ControllerBase
    {
        private readonly IFileService _files;

        public FileEntitiesController(IFileService files)
        {
            _files = files;
        }

        [HttpGet]
        public async Task<ActionResult<List<FileEntity>>> GetAll(CancellationToken ct)
        {
            var items = await _files.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FileEntity>> GetById(int id, CancellationToken ct)
        {
            var item = await _files.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("by-report/{reportId:int}")]
        public async Task<ActionResult<List<FileEntity>>> GetByReport(int reportId, CancellationToken ct)
        {
            var items = await _files.GetByReportIdAsync(reportId, ct);
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] FileEntity file, CancellationToken ct)
        {
            var id = await _files.CreateAsync(file, ct);
            return CreatedAtAction(nameof(GetById), new { id }, file);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] FileEntity file, CancellationToken ct)
        {
            if (id != file.Id) return BadRequest("Id mismatch.");

            try
            {
                await _files.UpdateAsync(file, ct);
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
                await _files.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
