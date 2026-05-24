using CleanCity.Infrastracture.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanCity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly IConfiguration _cfg;

    public AuthController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, IConfiguration cfg)
    {
        _users = users;
        _signIn = signIn;
        _cfg = cfg;
    }

    public record LoginRequest(string UserName, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _users.FindByNameAsync(req.UserName);
        if (user == null) return Unauthorized("Invalid credentials");

        var ok = await _signIn.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: false);
        if (!ok.Succeeded) return Unauthorized("Invalid credentials");

        var roles = await _users.GetRolesAsync(user);

        var token = CreateJwt(user, roles);
        return Ok(new
        {
            accessToken = token,
            userName = user.UserName,
            roles,
            driverId = user.DriverId
        });
    }

    private string CreateJwt(ApplicationUser user, IEnumerable<string> roles)
    {
        var jwt = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id),
        // التعديل هنا: استخدم ClaimTypes.Name لضمان ظهور الاسم في User.Identity.Name
        new(ClaimTypes.Name, user.UserName ?? ""),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // نصيحة: أضف معرف فريد للتوكن
    };

        foreach (var r in roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        if (user.DriverId.HasValue)
            claims.Add(new Claim("driverId", user.DriverId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresMinutes"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}