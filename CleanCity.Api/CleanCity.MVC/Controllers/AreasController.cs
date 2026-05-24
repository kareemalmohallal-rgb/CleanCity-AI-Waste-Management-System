using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.MVC.Controllers
{
    public class AreasController : Controller
    {
        private readonly IAreaService _areas;

        public AreasController(IAreaService areas)
        {
            _areas = areas;
        }

        // GET: /Areas
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _areas.GetAllForIndexAsync(ct);
            return View(items);
        }

        // GET: /Areas/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _areas.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /Areas/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Area());
        }

        // POST: /Areas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Area area, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(area);

            await _areas.CreateAsync(area, ct);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Areas/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _areas.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Areas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Area area, CancellationToken ct)
        {
            if (id != area.Id)
                return BadRequest("Id mismatch");

            if (!ModelState.IsValid)
                return View(area);

            try
            {
                await _areas.UpdateAsync(area, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(area);
            }
        }

        // GET: /Areas/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var item = await _areas.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                await _areas.DeleteAsync(id, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var item = await _areas.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", item);
            }
        }
    }
}