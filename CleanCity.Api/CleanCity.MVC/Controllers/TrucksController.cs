using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanCity.MVC.Controllers
{
    public class TrucksController : Controller
    {
        private readonly ITruckService _trucks;
        private readonly ITruckTypeService _types;
        private readonly IDriverService _drivers;

        public TrucksController(ITruckService trucks, ITruckTypeService types, IDriverService drivers)
        {
            _trucks = trucks;
            _types = types;
            _drivers = drivers;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _trucks.GetAllAsync(ct); // List<Truck>
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await FillDropdowns(ct);
            return View(new Truck());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Truck truck, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdowns(ct);
                return View(truck);
            }

            await _trucks.CreateAsync(truck, ct);
            return RedirectToAction(nameof(Index));
        }

        private async Task FillDropdowns(CancellationToken ct, int? selectedTypeId = null, int? selectedDriverId = null)
        {
            var types = await _types.GetAllAsync(ct);
            ViewBag.TruckTypes = types.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();

            var drivers = await _drivers.GetAllAsync(ct);
            ViewBag.Drivers = drivers.Select(d => new SelectListItem(d.FullName, d.Id.ToString())).ToList();
        }

        // GET: /Trucks/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _trucks.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var truck = await _trucks.GetByIdAsync(id, ct);
            if (truck == null) return NotFound();

            await FillDropdowns(ct, selectedTypeId: truck.TruckTypeId, selectedDriverId: truck.DriverId);
            return View(truck);
        }
        // POST: /Trucks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Truck truck, CancellationToken ct)
        {
            if (id != truck.Id)
                return BadRequest("Truck id mismatch.");

            if (!ModelState.IsValid)
            {
                await FillDropdowns(ct, truck.TruckTypeId, truck.DriverId);
                return View(truck);
            }

            try
            {
                await _trucks.UpdateAsync(truck, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(truck);
            }
        }

        // GET: /Trucks/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var item = await _trucks.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                await _trucks.DeleteAsync(id, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var item = await _trucks.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", item);
            }
        }
    }
}