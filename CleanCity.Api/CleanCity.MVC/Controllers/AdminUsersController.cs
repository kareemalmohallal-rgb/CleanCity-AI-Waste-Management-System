using CleanCity.Domain.Entities;
using CleanCity.Infrastracture.Identity;
using CleanCity.Infrastructure.Data;
using CleanCity.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.MVC.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly ApplicationContext _db;

        public AdminUsersController(UserManager<ApplicationUser> users, ApplicationContext db)
        {
            _users = users;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var usersInRole = await _users.GetUsersInRoleAsync("Driver");

            var driverIds = usersInRole
                .Where(u => u.DriverId.HasValue)
                .Select(u => u.DriverId!.Value)
                .ToList();

            var drivers = await _db.Drivers
                .Include(d => d.Area)
                .Include(d => d.Truck)
                .Where(d => driverIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, ct);

            var vm = usersInRole.Select(u =>
            {
                Driver? d = null;
                var isLinked = u.DriverId.HasValue && drivers.TryGetValue(u.DriverId.Value, out d);

                return new DriverUserVm
                {
                    UserId = u.Id,
                    UserName = u.UserName ?? "—",
                    DriverId = u.DriverId,
                    FullName = isLinked && d != null ? d.FullName : "—",
                    PhoneNumber = isLinked && d != null ? d.PhoneNumber : "—",
                    LicenseNumber = isLinked && d != null ? d.LicenseNumber : null,
                    AreaName = isLinked && d != null ? d.Area?.Name : null,
                    TruckName = isLinked && d != null ? d.Truck?.Name : null,
                    IsLinked = isLinked
                };
            }).OrderBy(v => v.FullName).ToList();

            return View(vm);
        }

        // GET: /AdminUsers/CreateUserRequest
        [HttpGet]
        public async Task<IActionResult> CreateUserRequest()
        {
            await LoadDriversAsync();
            return View(new CreateUserViewModel());
        }

        // POST: /AdminUsers/CreateUserRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserRequest(CreateUserViewModel model)
        {
            await LoadDriversAsync(model.DriverId);

            if (!ModelState.IsValid)
                return View(model);

            Driver? driver = null;

            // إذا تم تحديد سائق، نتحقق من وجوده
            if (model.DriverId.HasValue)
            {
                driver = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == model.DriverId.Value);
                if (driver == null)
                {
                    ModelState.AddModelError(nameof(model.DriverId), "السائق غير موجود");
                    return View(model);
                }

                // تحقق من عدم ربط السائق مسبقاً
                var userForDriver = await _users.Users.FirstOrDefaultAsync(u => u.DriverId == model.DriverId.Value);
                if (userForDriver != null)
                {
                    ModelState.AddModelError(nameof(model.DriverId), "هذا السائق مرتبط بمستخدم مسبقاً");
                    return View(model);
                }
            }

            // تحقق من اسم المستخدم
            var existingUser = await _users.FindByNameAsync(model.UserName.Trim());
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "اسم المستخدم مستخدم بالفعل");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName.Trim(),
                DriverId = model.DriverId,
                PhoneNumber = driver?.PhoneNumber
            };

            var createResult = await _users.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            // تحديد الصلاحية حسب وجود السائق
            var roleName = model.DriverId.HasValue ? "Driver" : "Admin";

            var roleResult = await _users.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, $"تم إنشاء المستخدم لكن فشل إضافة الدور ({roleName}): {error.Description}");

                return View(model);
            }

            TempData["Success"] = "تم إنشاء المستخدم بنجاح";
            return RedirectToAction(nameof(Index));
        }
        // GET: /AdminUsers/Edit/userId
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user == null) return NotFound();

            string fullName = "—";
            if (user.DriverId.HasValue)
            {
                var driver = await _db.Drivers.FindAsync(user.DriverId.Value);
                fullName = driver?.FullName ?? "—";
            }

            var vm = new EditDriverUserVm
            {
                UserId = user.Id,
                UserName = user.UserName ?? "",
                FullName = fullName
            };

            return View(vm);
        }

        // POST: /AdminUsers/Edit/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditDriverUserVm vm)
        {
            if (id != vm.UserId) return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var user = await _users.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.UserName != vm.UserName)
            {
                var setNameResult = await _users.SetUserNameAsync(user, vm.UserName);
                if (!setNameResult.Succeeded)
                {
                    foreach (var e in setNameResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return View(vm);
                }
            }

            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                var token = await _users.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _users.ResetPasswordAsync(user, token, vm.NewPassword);
                if (!resetResult.Succeeded)
                {
                    foreach (var e in resetResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return View(vm);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminUsers/Delete/userId
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user == null) return NotFound();

            string fullName = "—";
            if (user.DriverId.HasValue)
            {
                var driver = await _db.Drivers.FindAsync(user.DriverId.Value);
                fullName = driver?.FullName ?? "—";
            }

            var vm = new DriverUserVm
            {
                UserId = user.Id,
                UserName = user.UserName ?? "—",
                FullName = fullName,
                DriverId = user.DriverId
            };

            return View(vm);
        }

        // POST: /AdminUsers/Delete/userId
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _users.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var driver = user.DriverId.HasValue
                    ? await _db.Drivers.FindAsync(user.DriverId.Value)
                    : null;

                var vm = new DriverUserVm
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? "—",
                    FullName = driver?.FullName ?? "—",
                    DriverId = user.DriverId
                };

                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                return View("Delete", vm);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDriversAsync(int? selectedDriverId = null)
        {
            var drivers = await _db.Drivers
                .AsNoTracking()
                .Where(d => d.IsActive)
                .Select(d => new { d.Id, d.FullName })
                .OrderBy(d => d.FullName)
                .ToListAsync();

            ViewBag.Drivers = new SelectList(drivers, "Id", "FullName", selectedDriverId);
        }
    }
}