////using CleanCity.Application.Interfaces.Repositories;
////using CleanCity.Application.Interfaces.Services;
////using CleanCity.Application.Services;
////using CleanCity.Infrastracture.Identity;
////using CleanCity.Infrastructure; // مهم (AddApplication / AddInfrastructure حسب مشروعك)
////using CleanCity.Infrastructure.AI;
////using CleanCity.Infrastructure.Identity; // ApplicationUser + ApplicationIdentityDbContext
////using CleanCity.Infrastructure.Notifications;
////using CleanCity.Infrastructure.Persistence.Repositories;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.EntityFrameworkCore;
////using FirebaseAdmin;
////using Google.Apis.Auth.OAuth2;

////namespace CleanCity.MVC
////{
////    public class Program
////    {
////        public static async Task Main(string[] args)
////        {
////            var builder = WebApplication.CreateBuilder(args);

////            // =========================
////            // MVC
////            // =========================
////            builder.Services.AddControllersWithViews();

////            // =========================
////            // Application services (الموجودة عندك)
////            // =========================
////            builder.Services.AddScoped<IDriverService, DriverService>();
////            builder.Services.AddScoped<ITruckService, TruckService>();
////            builder.Services.AddScoped<IAreaService, AreaService>();
////            builder.Services.AddScoped<ISewageServiceProviderService, SewageServiceProviderService>();
////            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();
////            builder.Services.AddScoped<ITruckTypeService, TruckTypeService>();
////            builder.Services.AddScoped<IReportAssignmentService, ReportAssignmentService>();
////            builder.Services.AddScoped<IReportService, ReportService>();
////            builder.Services.AddScoped<IWasteImageValidationService, PythonWasteImageValidationService>();
////            builder.Services.AddScoped<IAnonymousDeviceService, AnonymousDeviceService>();
////            builder.Services.AddScoped<IReportStatusHistoryService, ReportStatusHistoryService>();
////            builder.Services.AddScoped<IAIAnalysisResultService, AIAnalysisResultService>();
////            builder.Services.AddScoped<IFileService, FileService>();
////            builder.Services.AddScoped<INotificationService, NotificationService>();
////            builder.Services.AddScoped<IPushNotificationSender, FirebasePushNotificationSender>();
////            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();

////            // =========================
////            // Infrastructure (DbContext + Repos + UoW + ... )
////            // ملاحظة: استخدم AddApplication فقط إذا كان هذا هو الاسم الصحيح في مشروعك.
////            // إذا عندك AddInfrastructure / AddApplicationServices فاستبدلها بالأسماء الصحيحة.
////            // =========================
////            builder.Services.AddInfrastructure(builder.Configuration);

////            // =========================
////            // Identity DbContext (خاص بالمستخدمين/الهوية)
////            // IMPORTANT:
////            // تأكد من وجود ConnectionStrings:DefaultConnection في appsettings.json
////            // =========================
////            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
////                options.UseSqlServer(
////                    builder.Configuration.GetConnectionString("DefaultConnection")));

////            // =========================
////            // ASP.NET Identity
////            // =========================
////            builder.Services
////                .AddIdentity<ApplicationUser, IdentityRole>(options =>
////                {
////                    // Password policy (يمكن تعديلها حسب رغبتك)
////                    options.Password.RequiredLength = 6;
////                    options.Password.RequireDigit = false;
////                    options.Password.RequireUppercase = false;
////                    options.Password.RequireLowercase = false;
////                    options.Password.RequireNonAlphanumeric = false;

////                    // User settings
////                    options.User.RequireUniqueEmail = false; // لأنك تستخدم UserName فقط غالباً
////                })
////                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
////                .AddDefaultTokenProviders();

////            // =========================
////            // Authentication Cookie (مفيد لتسجيل الدخول في MVC)
////            // =========================
////            builder.Services.ConfigureApplicationCookie(options =>
////            {
////                options.LoginPath = "/Login/Index"; ;
////                options.AccessDeniedPath = "/Account/AccessDenied";
////                options.LogoutPath = "/Account/Logout";
////                options.Cookie.Name = "CleanCity.Auth";
////                options.SlidingExpiration = true;
////                options.ExpireTimeSpan = TimeSpan.FromHours(8);
////            });

////            // =========================
////            // Session (اختياري)
////            // =========================
////            builder.Services.AddDistributedMemoryCache();
////            builder.Services.AddSession(options =>
////            {
////                options.Cookie.HttpOnly = true;
////                options.Cookie.IsEssential = true;
////                options.IdleTimeout = TimeSpan.FromMinutes(30);
////            });
////            var firebaseCredentialsPath = builder.Configuration["Firebase:ServiceAccountPath"];
////            if (string.IsNullOrWhiteSpace(firebaseCredentialsPath))
////                throw new InvalidOperationException("Firebase:ServiceAccountPath is missing.");

////            if (FirebaseApp.DefaultInstance == null)
////            {
////                FirebaseApp.Create(new AppOptions
////                {
////                    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
////                });
////            }

////            builder.Services.AddSingleton<IPushNotificationSender, FirebasePushNotificationSender>();

////            var app = builder.Build();

////            // =========================
////            // Seed Roles/User (اختياري لكن مفيد)
////            // ينشئ الأدوار الأساسية إن لم تكن موجودة
////            // =========================
////            await SeedIdentityAsync(app);

////            // =========================
////            // Middleware pipeline
////            // =========================
////            if (!app.Environment.IsDevelopment())
////            {
////                app.UseExceptionHandler("/Home/Error");
////                app.UseHsts();
////            }

////            app.UseHttpsRedirection();
////            app.UseStaticFiles();

////            // ✅ يُقدّم ملفات API/wwwroot — لعرض صور البلاغات المرفوعة من Flutter
////            app.UseStaticFiles(new StaticFileOptions
////            {
////                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
////                    Path.Combine(
////                        builder.Environment.ContentRootPath,
////                        "..",
////                        "CleanCity.Api",
////                        "wwwroot"
////                    )
////                ),
////                RequestPath = ""
////            });

////            app.UseRouting();

////            app.UseSession();

////            // مهم جداً: Authentication قبل Authorization
////            app.UseAuthentication();
////            app.UseAuthorization();

////            app.MapControllerRoute(
////                name: "default",
////                pattern: "{controller=Home}/{action=Index}/{id?}");
////               // pattern: "{controller=login}/{action=Index}/{id?}");

////            app.Run();
////        }

////        // =========================
////        // Seed Identity Roles/Admin
////        // =========================
////        private static async Task SeedIdentityAsync(WebApplication app)
////        {
////            using var scope = app.Services.CreateScope();

////            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
////            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

////            var roles = new[] { "Admin", "Driver" };

////            foreach (var role in roles)
////            {
////                if (!await roleManager.RoleExistsAsync(role))
////                {
////                    await roleManager.CreateAsync(new IdentityRole(role));
////                }
////            }

////            // إنشاء أدمن افتراضي (اختياري)
////            const string adminUserName = "admin";
////            const string adminPassword = "Admin@12345";

////            var admin = await userManager.FindByNameAsync(adminUserName);
////            if (admin is null)
////            {
////                admin = new ApplicationUser
////                {
////                    UserName = adminUserName
////                    // Email = "admin@cleancity.local" // اختياري
////                };

////                var createResult = await userManager.CreateAsync(admin, adminPassword);

////                if (createResult.Succeeded)
////                {
////                    await userManager.AddToRoleAsync(admin, "Admin");
////                }
////                else
////                {
////                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
////                    throw new InvalidOperationException($"Failed to seed admin user: {errors}");
////                }
////            }
////        }
////    }
////}

////النسخة الثانية
////using CleanCity.Application.Interfaces.Repositories;
////using CleanCity.Application.Interfaces.Services;
////using CleanCity.Application.Services;
////using CleanCity.Infrastracture.Identity;
////using CleanCity.Infrastructure;
////using CleanCity.Infrastructure.AI;
////using CleanCity.Infrastructure.Identity;
////using CleanCity.Infrastructure.Notifications;
////using CleanCity.Infrastructure.Persistence.Repositories;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.EntityFrameworkCore;
////using FirebaseAdmin;
////using Google.Apis.Auth.OAuth2;

////namespace CleanCity.MVC
////{
////    public class Program
////    {
////        public static async Task Main(string[] args)
////        {
////            var builder = WebApplication.CreateBuilder(args);

////            builder.Services.AddControllersWithViews();

////            builder.Services.AddScoped<IDriverService, DriverService>();
////            builder.Services.AddScoped<ITruckService, TruckService>();
////            builder.Services.AddScoped<IAreaService, AreaService>();
////            builder.Services.AddScoped<ISewageServiceProviderService, SewageServiceProviderService>();
////            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();
////            builder.Services.AddScoped<ITruckTypeService, TruckTypeService>();
////            builder.Services.AddScoped<IReportAssignmentService, ReportAssignmentService>();
////            builder.Services.AddScoped<IReportService, ReportService>();
////            builder.Services.AddScoped<IWasteImageValidationService, PythonWasteImageValidationService>();
////            builder.Services.AddScoped<IAnonymousDeviceService, AnonymousDeviceService>();
////            builder.Services.AddScoped<IReportStatusHistoryService, ReportStatusHistoryService>();
////            builder.Services.AddScoped<IAIAnalysisResultService, AIAnalysisResultService>();
////            builder.Services.AddScoped<IFileService, FileService>();
////            builder.Services.AddScoped<INotificationService, NotificationService>();
////            builder.Services.AddScoped<IPushNotificationSender, FirebasePushNotificationSender>();
////            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();

////            builder.Services.AddInfrastructure(builder.Configuration);

////            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
////                options.UseSqlServer(
////                    builder.Configuration.GetConnectionString("DefaultConnection")));

////            builder.Services
////                .AddIdentity<ApplicationUser, IdentityRole>(options =>
////                {
////                    options.Password.RequiredLength = 6;
////                    options.Password.RequireDigit = false;
////                    options.Password.RequireUppercase = false;
////                    options.Password.RequireLowercase = false;
////                    options.Password.RequireNonAlphanumeric = false;
////                    options.User.RequireUniqueEmail = false;
////                })
////                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
////                .AddDefaultTokenProviders();

////            builder.Services.ConfigureApplicationCookie(options =>
////            {
////                options.LoginPath = "/Login/Index";
////                options.AccessDeniedPath = "/Account/AccessDenied";
////                options.LogoutPath = "/Account/Logout";
////                options.Cookie.Name = "CleanCity.Auth";
////                options.SlidingExpiration = true;
////                options.ExpireTimeSpan = TimeSpan.FromHours(8);
////            });

////            builder.Services.AddDistributedMemoryCache();
////            builder.Services.AddSession(options =>
////            {
////                options.Cookie.HttpOnly = true;
////                options.Cookie.IsEssential = true;
////                options.IdleTimeout = TimeSpan.FromMinutes(30);
////            });

////            var firebaseCredentialsPath = builder.Configuration["Firebase:ServiceAccountPath"];
////            if (string.IsNullOrWhiteSpace(firebaseCredentialsPath))
////                throw new InvalidOperationException("Firebase:ServiceAccountPath is missing.");

////            if (FirebaseApp.DefaultInstance == null)
////            {
////                FirebaseApp.Create(new AppOptions
////                {
////                    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
////                });
////            }

////            builder.Services.AddSingleton<IPushNotificationSender, FirebasePushNotificationSender>();

////            var app = builder.Build();

////            await SeedIdentityAsync(app);

////            if (!app.Environment.IsDevelopment())
////            {
////                app.UseExceptionHandler("/Home/Error");
////                app.UseHsts();
////            }

////            app.UseHttpsRedirection();

////            // ✅ يُقدّم ملفات wwwroot الخاصة بالـ MVC
////            app.UseStaticFiles();

////            // ✅ يُقدّم ملفات API/wwwroot — لعرض صور البلاغات المرفوعة من Flutter
////            app.UseStaticFiles(new StaticFileOptions
////            {
////                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
////                    Path.Combine(
////                        builder.Environment.ContentRootPath,
////                        "..",
////                        "CleanCity.Api",
////                        "wwwroot"
////                    )
////                ),
////                RequestPath = ""
////            });

////            app.UseRouting();
////            app.UseSession();
////            app.UseAuthentication();
////            app.UseAuthorization();

////            app.MapControllerRoute(
////                name: "default",
////                pattern: "{controller=Home}/{action=Index}/{id?}");

////            app.Run();
////        }

////        private static async Task SeedIdentityAsync(WebApplication app)
////        {
////            using var scope = app.Services.CreateScope();

////            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
////            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

////            var roles = new[] { "Admin", "Driver" };

////            foreach (var role in roles)
////            {
////                if (!await roleManager.RoleExistsAsync(role))
////                {
////                    await roleManager.CreateAsync(new IdentityRole(role));
////                }
////            }

////            const string adminUserName = "admin";
////            const string adminPassword = "Admin@12345";

////            var admin = await userManager.FindByNameAsync(adminUserName);
////            if (admin is null)
////            {
////                admin = new ApplicationUser { UserName = adminUserName };

////                var createResult = await userManager.CreateAsync(admin, adminPassword);

////                if (createResult.Succeeded)
////                {
////                    await userManager.AddToRoleAsync(admin, "Admin");
////                }
////                else
////                {
////                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
////                    throw new InvalidOperationException($"Failed to seed admin user: {errors}");
////                }
////            }
////        }
////    }
////}

//using CleanCity.Application.Interfaces.Repositories;
//using CleanCity.Application.Interfaces.Services;
//using CleanCity.Application.Services;
//using CleanCity.Infrastracture.Identity;
//using CleanCity.Infrastructure;
//using CleanCity.Infrastructure.AI;
//using CleanCity.Infrastructure.Identity;
//using CleanCity.Infrastructure.Notifications;
//using CleanCity.Infrastructure.Persistence.Repositories;
//using CleanCity.MVC.Hubs;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using FirebaseAdmin;
//using Google.Apis.Auth.OAuth2;

//namespace CleanCity.MVC
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            builder.Services.AddControllersWithViews();

//            // ── SignalR ──────────────────────────────────────────────────────────
//            builder.Services.AddSignalR();

//            builder.Services.AddScoped<IDriverService, DriverService>();
//            builder.Services.AddScoped<ITruckService, TruckService>();
//            builder.Services.AddScoped<IAreaService, AreaService>();
//            builder.Services.AddScoped<ISewageServiceProviderService, SewageServiceProviderService>();
//            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();
//            builder.Services.AddScoped<ITruckTypeService, TruckTypeService>();
//            builder.Services.AddScoped<IReportAssignmentService, ReportAssignmentService>();
//            builder.Services.AddScoped<IReportService, ReportService>();
//            builder.Services.AddScoped<IWasteImageValidationService, PythonWasteImageValidationService>();
//            builder.Services.AddScoped<IAnonymousDeviceService, AnonymousDeviceService>();
//            builder.Services.AddScoped<IReportStatusHistoryService, ReportStatusHistoryService>();
//            builder.Services.AddScoped<IAIAnalysisResultService, AIAnalysisResultService>();
//            builder.Services.AddScoped<IFileService, FileService>();
//            builder.Services.AddScoped<INotificationService, NotificationService>();
//            builder.Services.AddScoped<IPushNotificationSender, FirebasePushNotificationSender>();
//            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();

//            builder.Services.AddInfrastructure(builder.Configuration);

//            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
//                options.UseSqlServer(
//                    builder.Configuration.GetConnectionString("DefaultConnection")));

//            builder.Services
//                .AddIdentity<ApplicationUser, IdentityRole>(options =>
//                {
//                    options.Password.RequiredLength = 6;
//                    options.Password.RequireDigit = false;
//                    options.Password.RequireUppercase = false;
//                    options.Password.RequireLowercase = false;
//                    options.Password.RequireNonAlphanumeric = false;
//                    options.User.RequireUniqueEmail = false;
//                })
//                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
//                .AddDefaultTokenProviders();

//            builder.Services.ConfigureApplicationCookie(options =>
//            {
//                options.LoginPath = "/Login/Index";
//                options.AccessDeniedPath = "/Account/AccessDenied";
//                options.LogoutPath = "/Account/Logout";
//                options.Cookie.Name = "CleanCity.Auth";
//                options.SlidingExpiration = true;
//                options.ExpireTimeSpan = TimeSpan.FromHours(8);
//            });

//            builder.Services.AddDistributedMemoryCache();
//            builder.Services.AddSession(options =>
//            {
//                options.Cookie.HttpOnly = true;
//                options.Cookie.IsEssential = true;
//                options.IdleTimeout = TimeSpan.FromMinutes(30);
//            });

//            var firebaseCredentialsPath = builder.Configuration["Firebase:ServiceAccountPath"];
//            if (string.IsNullOrWhiteSpace(firebaseCredentialsPath))
//                throw new InvalidOperationException("Firebase:ServiceAccountPath is missing.");

//            if (FirebaseApp.DefaultInstance == null)
//            {
//                FirebaseApp.Create(new AppOptions
//                {
//                    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
//                });
//            }

//            builder.Services.AddSingleton<IPushNotificationSender, FirebasePushNotificationSender>();

//            var app = builder.Build();

//            await SeedIdentityAsync(app);

//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();

//            // ✅ يُقدّم ملفات wwwroot الخاصة بالـ MVC
//            app.UseStaticFiles();

//            // ✅ يُقدّم ملفات API/wwwroot — لعرض صور البلاغات المرفوعة من Flutter
//            app.UseStaticFiles(new StaticFileOptions
//            {
//                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
//                    Path.Combine(
//                        builder.Environment.ContentRootPath,
//                        "..",
//                        "CleanCity.Api",
//                        "wwwroot"
//                    )
//                ),
//                RequestPath = ""
//            });

//            app.UseRouting();
//            app.UseSession();
//            app.UseAuthentication();
//            app.UseAuthorization();

//            // ── SignalR Hub endpoint ─────────────────────────────────────────────
//            app.MapHub<DashboardHub>("/hubs/dashboard");

//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");

//            app.Run();
//        }

//        private static async Task SeedIdentityAsync(WebApplication app)
//        {
//            using var scope = app.Services.CreateScope();

//            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//            var roles = new[] { "Admin", "Driver" };

//            foreach (var role in roles)
//            {
//                if (!await roleManager.RoleExistsAsync(role))
//                {
//                    await roleManager.CreateAsync(new IdentityRole(role));
//                }
//            }

//            const string adminUserName = "admin";
//            const string adminPassword = "Admin@12345";

//            var admin = await userManager.FindByNameAsync(adminUserName);
//            if (admin is null)
//            {
//                admin = new ApplicationUser { UserName = adminUserName };

//                var createResult = await userManager.CreateAsync(admin, adminPassword);

//                if (createResult.Succeeded)
//                {
//                    await userManager.AddToRoleAsync(admin, "Admin");
//                }
//                else
//                {
//                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
//                    throw new InvalidOperationException($"Failed to seed admin user: {errors}");
//                }
//            }
//        }
//    }
//}

using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Application.Services;
using CleanCity.Infrastracture.Identity;
using CleanCity.Infrastructure;
using CleanCity.Infrastructure.AI;
using CleanCity.Infrastructure.Identity;
using CleanCity.Infrastructure.Notifications;
using CleanCity.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace CleanCity.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ [Authorize] عالمي: كل الصفحات تتطلب تسجيل دخول تلقائياً
            // LoginController فيها [AllowAnonymous] لتجاوز هذا القيد
            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddScoped<IDriverService, DriverService>();
            builder.Services.AddScoped<ITruckService, TruckService>();
            builder.Services.AddScoped<IAreaService, AreaService>();
            builder.Services.AddScoped<ISewageServiceProviderService, SewageServiceProviderService>();
            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();
            builder.Services.AddScoped<ITruckTypeService, TruckTypeService>();
            builder.Services.AddScoped<IReportAssignmentService, ReportAssignmentService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IWasteImageValidationService, PythonWasteImageValidationService>();
            builder.Services.AddScoped<IAnonymousDeviceService, AnonymousDeviceService>();
            builder.Services.AddScoped<IReportStatusHistoryService, ReportStatusHistoryService>();
            builder.Services.AddScoped<IAIAnalysisResultService, AIAnalysisResultService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IPushNotificationSender, FirebasePushNotificationSender>();
            builder.Services.AddScoped<IAwarenessContentService, AwarenessContentService>();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            // ✅ عند فتح التطبيق:
            //    - إذا الـ Cookie موجودة وصالحة → يكمل للصفحة المطلوبة
            //    - إذا غير مسجل دخول → يُعاد توجيهه لـ /Login/Index
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Index";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Login/Logout";
                options.Cookie.Name = "CleanCity.Auth";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
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

            await SeedIdentityAsync(app);

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // ✅ يُقدّم ملفات wwwroot الخاصة بالـ MVC
            app.UseStaticFiles();

            // ✅ يُقدّم ملفات API/wwwroot — لعرض صور البلاغات المرفوعة من Flutter
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Path.Combine(
                        builder.Environment.ContentRootPath,
                        "..",
                        "CleanCity.Api",
                        "wwwroot"
                    )
                ),
                RequestPath = ""
            });

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // ✅ المسار الافتراضي = Login/Index
            // غير مسجل دخول → Login/Index مباشرة
            // مسجل دخول → Login/Index [GET] يعيد توجيهه لـ Home/Index تلقائياً
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }

        private static async Task SeedIdentityAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = new[] { "Admin", "Driver" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            const string adminUserName = "admin";
            const string adminPassword = "Admin@12345";

            var admin = await userManager.FindByNameAsync(adminUserName);
            if (admin is null)
            {
                admin = new ApplicationUser { UserName = adminUserName };

                var createResult = await userManager.CreateAsync(admin, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed admin user: {errors}");
                }
            }
        }
    }
}