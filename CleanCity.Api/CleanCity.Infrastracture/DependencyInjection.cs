
using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Application.Interfaces.Storage;
using CleanCity.Infrastructure.AI;
using CleanCity.Infrastructure.Data;
using CleanCity.Infrastructure.Persistence;
using CleanCity.Infrastructure.Persistence.Repositories;
using CleanCity.Infrastructure.Repositories;
using CleanCity.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CleanCity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationContext>(options =>
               options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();             // إذا مستخدم
            services.AddScoped<ITruckRepository, TruckRepository>();
            services.AddScoped<ITruckTypeRepository, TruckTypeRepository>();
            services.AddScoped<ISewageServiceProviderRepository, SewageServiceProviderRepository>();
            services.AddScoped<IAwarenessContentRepository, AwarenessContentRepository>();
            services.AddScoped<IReportAssignmentRepository, ReportAssignmentRepository>();
            services.AddScoped<IAnonymousDeviceRepository, AnonymousDeviceRepository>();
            services.AddScoped<IAIAnalysisResultRepository, AIAnalysisResultRepository>();
            services.AddScoped<IReportStatusHistoryRepository, ReportStatusHistoryRepository>();
            services.AddScoped<IFileRepository, FileRepository > ();
            services.AddScoped<IAwarenessContentRepository, AwarenessContentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationRecipientRepository, NotificationRecipientRepository>();
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(uploadsRoot, "/uploads"));
            services.AddHttpClient<IWasteImageValidationService, PythonWasteImageValidationService>();
            return services;
        }
    }
}
