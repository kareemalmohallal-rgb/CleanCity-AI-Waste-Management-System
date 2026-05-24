using CleanCity.Infrastracture.Identity;
using CleanCity.Infrastructure.Data; // إذا كنت تريد التحقق من وجود السائق من جدول Drivers
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly ApplicationContext _db; // DbContext الدومين (Drivers)

    public AdminUsersController(UserManager<ApplicationUser> users, ApplicationContext db)
    {
        _users = users;
        _db = db;
    }

    public record CreateDriverUserRequest(int DriverId, string UserName, string Password);
    //[HttpGet("ping")]
    //[AllowAnonymous]
    //public IActionResult Ping() => Ok("admin ok");
    [HttpGet("whoami")]
    public IActionResult WhoAmI() =>
    Ok(new { name = User.Identity?.Name, roles = User.Claims.Where(c => c.Type.EndsWith("/role") || c.Type.Contains("role")).Select(c => c.Value) });
    [HttpPost("driver-users")]
    public async Task<IActionResult> CreateDriverUser(CreateDriverUserRequest req, CancellationToken ct)
    {
        // 1) تحقق من وجود السائق
        var driverExists = await _db.Drivers.AnyAsync(d => d.Id == req.DriverId, ct);
        if (!driverExists)
            return BadRequest($"DriverId={req.DriverId} غير موجود.");

        // 2) تحقق أن هذا السائق ليس مرتبطاً بحساب سابقاً
        var alreadyLinked = await _users.Users.AnyAsync(u => u.DriverId == req.DriverId, ct);
        if (alreadyLinked)
            return Conflict("هذا السائق مرتبط بحساب بالفعل.");

        // 3) تحقق أن اسم المستخدم غير مستخدم (نستخدم رقم الهاتف غالباً)
        var existingUser = await _users.FindByNameAsync(req.UserName);
        if (existingUser != null)
            return Conflict("اسم المستخدم مستخدم بالفعل.");

        // 4) إنشاء المستخدم وربطه بالسائق
        var user = new ApplicationUser
        {
            UserName = req.UserName.Trim(),
            PhoneNumber = req.UserName.Trim(),
            DriverId = req.DriverId
        };

        var createRes = await _users.CreateAsync(user, req.Password);
        if (!createRes.Succeeded)
            return BadRequest(new
            {
                message = "فشل إنشاء المستخدم",
                errors = createRes.Errors.Select(e => e.Description).ToList()
            });

        // 5) إضافته لدور Driver
        var roleRes = await _users.AddToRoleAsync(user, "Driver");
        if (!roleRes.Succeeded)
            return BadRequest(new
            {
                message = "تم إنشاء المستخدم لكن فشل تعيين الدور",
                errors = roleRes.Errors.Select(e => e.Description).ToList()
            });

        return Ok(new
        {
            userId = user.Id,
            userName = user.UserName,
            driverId = user.DriverId,
            role = "Driver"
        });
    }
}