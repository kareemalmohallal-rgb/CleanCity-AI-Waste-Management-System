//using CleanCity.Api.Extensions;
//using CleanCity.Application.Interfaces.Services;
//using CleanCity.Infrastracture.Identity;
//using CleanCity.Infrastructure.AI;
//using CleanCity.Infrastructure.Identity;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Security.Claims;
//using System.Text;
//using FirebaseAdmin;
//using Google.Apis.Auth.OAuth2;
//using CleanCity.Infrastructure.Notifications;
//var builder = WebApplication.CreateBuilder(args);

//// =======================
//// Services
//// =======================

//builder.Services
//    .AddInfrastructure(builder.Configuration)
//    .AddApplicationServices();

//CleanCity.Infrastructure.DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration);

//builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddHttpClient<IWasteImageValidationService, PythonWasteImageValidationService>(client =>
//{
//    client.BaseAddress = new Uri("http://127.0.0.1:8001");
//    client.Timeout = TimeSpan.FromSeconds(30);
//});
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//{
//    options.Password.RequiredLength = 8;
//    options.Password.RequireDigit = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//})
//    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
//    .AddDefaultTokenProviders();

//// JWT Configuration
//var jwt = builder.Configuration.GetSection("Jwt");
//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddJwtBearer(options =>
//    {
//        // تم إزالة MapInboundClaims = false للسماح بالتحويل التلقائي للـ Claims
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,

//            ValidIssuer = jwt["Issuer"],
//            ValidAudience = jwt["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing"))
//            ),
//            ClockSkew = TimeSpan.Zero,

//            // ضبط الأنواع القياسية للـ Claims لضمان عمل User.Identity.Name و User.IsInRole
//            NameClaimType = ClaimTypes.Name,
//            RoleClaimType = ClaimTypes.Role
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnTokenValidated = context =>
//            {
//                Console.WriteLine("=== JWT TOKEN VALIDATED ===");
//                return Task.CompletedTask;
//            },
//            OnAuthenticationFailed = context =>
//            {
//                Console.WriteLine("=== JWT AUTH FAILED ===");
//                Console.WriteLine(context.Exception.ToString());
//                return Task.CompletedTask;
//            }
//        };
//    });

//builder.Services.AddAuthorization();
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "CleanCity API", Version = "v1" });

//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Description = "اكتب التوكن فقط (بدون كلمة Bearer إذا كان Swagger يضيفها تلقائياً)"
//    });

//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader());
//});
//var firebaseCredentialsPath = builder.Configuration["Firebase:ServiceAccountPath"];
//if (string.IsNullOrWhiteSpace(firebaseCredentialsPath))
//    throw new InvalidOperationException("Firebase:ServiceAccountPath is missing.");

//if (FirebaseApp.DefaultInstance == null)
//{
//    FirebaseApp.Create(new AppOptions
//    {
//        Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
//    });
//}

//builder.Services.AddSingleton<IPushNotificationSender, FirebasePushNotificationSender>();
//var app = builder.Build();

//// =======================
//// Seed Identity
//// =======================
//await SeedIdentityAsync(app);

//// =======================
//// Middleware (الترتيب مهم جداً هنا)
//// =======================
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseStaticFiles();   // ? مهم جداً لعرض الصور من wwwroot
//app.UseHttpsRedirection();
//app.UseCors("AllowAll");

//app.UseAuthentication(); // يجب أن يسبق UseAuthorization
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

//// =======================
//// Seed Identity Function
//// =======================
//static async Task SeedIdentityAsync(WebApplication app)
//{
//    using var scope = app.Services.CreateScope();
//    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//    foreach (var role in new[] { "Admin", "Driver" })
//    {
//        if (!await roleMgr.RoleExistsAsync(role))
//            await roleMgr.CreateAsync(new IdentityRole(role));
//    }

//    const string adminUserName = "admin";
//    var admin = await userMgr.FindByNameAsync(adminUserName);

//    if (admin is null)
//    {
//        admin = new ApplicationUser { UserName = adminUserName };
//        var create = await userMgr.CreateAsync(admin, "Admin@12345");

//        if (create.Succeeded)
//            await userMgr.AddToRoleAsync(admin, "Admin");
//    }
//}

using CleanCity.Api.Extensions;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Infrastracture.Identity;
using CleanCity.Infrastructure.AI;
using CleanCity.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using CleanCity.Infrastructure.Notifications;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Services
// =======================

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplicationServices();

CleanCity.Infrastructure.DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration);

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IWasteImageValidationService, PythonWasteImageValidationService>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8001");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();

// JWT Configuration
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing"))
            ),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                Console.WriteLine("=== JWT TOKEN VALIDATED ===");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("=== JWT AUTH FAILED ===");
                Console.WriteLine(context.Exception.ToString());
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CleanCity API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "ادخل التوكن هنا"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var firebaseCredentialsPath = builder.Configuration["Firebase:ServiceAccountPath"];
if (string.IsNullOrWhiteSpace(firebaseCredentialsPath))
    throw new InvalidOperationException("Firebase:ServiceAccountPath is missing.");

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
    });
}

builder.Services.AddSingleton<IPushNotificationSender, FirebasePushNotificationSender>();

var app = builder.Build();

// =======================
// Seed Identity
// =======================
await SeedIdentityAsync(app);

// =======================
// Middleware
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ يُقدّم ملفات wwwroot الخاصة بالـ API
app.UseStaticFiles();

// ✅ يُقدّم ملفات MVC/wwwroot تلقائياً — أي صورة تُرفع من MVC تظهر فوراً في Flutter
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(
            builder.Environment.ContentRootPath,
            "..",
            "CleanCity.MVC",
            "wwwroot"
        )
    ),
    RequestPath = ""
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// =======================
// Seed Identity Function
// =======================
static async Task SeedIdentityAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { "Admin", "Driver" })
    {
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));
    }

    const string adminUserName = "admin";
    var admin = await userMgr.FindByNameAsync(adminUserName);

    if (admin is null)
    {
        admin = new ApplicationUser { UserName = adminUserName };
        var create = await userMgr.CreateAsync(admin, "Admin@12345");

        if (create.Succeeded)
            await userMgr.AddToRoleAsync(admin, "Admin");
    }
}