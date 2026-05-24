using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using CleanCity.Infrastracture.Identity;
using CleanCity.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.MVC.Controllers
{
    public class DriversController : Controller
    {
        private readonly IDriverService _drivers;
        private readonly IAreaService _areas;
        private readonly ITruckService _trucks;
        private readonly UserManager<ApplicationUser> _users;

        public DriversController(
            IDriverService drivers,
            IAreaService areas,
            ITruckService trucks,
            UserManager<ApplicationUser> users)
        {
            _drivers = drivers;
            _areas = areas;
            _trucks = trucks;
            _users = users;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var drivers = await _drivers.GetAllAsync(ct);

            // جلب كل مستخدمي دور Driver مع DriverId
            var driverUsers = await _users.GetUsersInRoleAsync("Driver");
            var userMap = driverUsers
                .Where(u => u.DriverId.HasValue)
                .ToDictionary(u => u.DriverId!.Value, u => u.UserName ?? "—");

            var vm = drivers.Select(d => new DriverIndexVm
            {
                Id = d.Id,
                FullName = d.FullName,
                UserName = userMap.TryGetValue(d.Id, out var uname) ? uname : "—",
                AreaName = d.Area?.Name,
                TruckName = d.Truck?.Name,
                IsActive = d.IsActive
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await FillDropdowns(ct);
            return View(new Driver { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Driver driver, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdowns(ct);
                return View(driver);
            }

            await _drivers.CreateAsync(driver, ct);
            return RedirectToAction(nameof(Index));
        }

        private async Task FillDropdowns(CancellationToken ct, int? selectedAreaId = null, int? selectedTruckId = null)
        {
            var areas = await _areas.GetAllAsync(ct);
            ViewBag.Areas = areas.Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToList();

            var trucks = await _trucks.GetAllAsync(ct);
            ViewBag.Trucks = trucks.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var driver = await _drivers.GetByIdAsync(id, ct);
            if (driver == null) return NotFound();
            return View(driver);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var driver = await _drivers.GetByIdAsync(id, ct);
            if (driver == null) return NotFound();
            await FillDropdowns(ct, driver.AreaId, driver.TruckId);
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Driver driver, CancellationToken ct)
        {
            if (id != driver.Id) return BadRequest("Driver is mismatch.");

            if (!ModelState.IsValid)
            {
                await FillDropdowns(ct, driver.AreaId, driver.TruckId);
                return View(driver);
            }

            try
            {
                await _drivers.UpdateAsync(driver, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await FillDropdowns(ct, driver.AreaId, driver.TruckId);
                return View(driver);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var driver = await _drivers.GetByIdAsync(id, ct);
            if (driver == null) return NotFound();
            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                await _drivers.DeleteAsync(id, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                var driver = await _drivers.GetByIdAsync(id, ct);
                if (driver == null) return NotFound();

                ModelState.AddModelError(string.Empty,
                    "لا يمكن حذف هذا السائق لأنه مرتبط بتكليفات موجودة. يرجى إلغاء التكليفات أولاً.");
                return View("Delete", driver);
            }
            catch (InvalidOperationException ex)
            {
                var driver = await _drivers.GetByIdAsync(id, ct);
                if (driver == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", driver);
            }
        }
    }
}