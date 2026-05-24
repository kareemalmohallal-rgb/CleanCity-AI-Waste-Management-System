using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.MVC.Controllers
{
    public class TruckTypesController : Controller
    {
        private readonly ITruckTypeService _types;

        public TruckTypesController(ITruckTypeService types)
        {
            _types = types;
        }

        // GET: /TruckTypes
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _types.GetAllAsync(ct);
            return View(items);
        }

        // GET: /TruckTypes/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _types.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /TruckTypes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TruckType());
        }

        // POST: /TruckTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TruckType type, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(type);

            await _types.CreateAsync(type, ct);
            return RedirectToAction(nameof(Index));
        }

        // GET: /TruckTypes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _types.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /TruckTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TruckType type, CancellationToken ct)
        {
            if (id != type.Id)
                return BadRequest("TruckType id mismatch.");

            if (!ModelState.IsValid)
                return View(type);

            try
            {
                await _types.UpdateAsync(type, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(type);
            }
        }

        // GET: /TruckTypes/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var item = await _types.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /TruckTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                await _types.DeleteAsync(id, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var item = await _types.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", item);
            }
        }
    }
}