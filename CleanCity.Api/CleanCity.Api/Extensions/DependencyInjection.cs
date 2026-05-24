using CleanCity.Application.Interfaces;
using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Application.Services;
using CleanCity.Infrastructure.Data;
using CleanCity.Infrastructure.Persistence;
using CleanCity.Infrastructure.Persistence.Repositories;
using CleanCity.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ITruckRepository, TruckRepository>();
            services.AddScoped<ITruckTypeRepository, TruckTypeRepository>();
            services.AddScoped<ISewageServiceProviderRepository, SewageServiceProviderRepository>();
            services.AddScoped<IAwarenessContentRepository, AwarenessContentRepository>();
            services.AddScoped<IAnonymousDeviceRepository, AnonymousDeviceRepository>();
            //services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IAIAnalysisResultRepository, AIAnalysisResultRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportAssignmentRepository, ReportAssignmentRepository>();
            services.AddScoped<IReportStatusHistoryRepository, ReportStatusHistoryRepository>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITruckService, TruckService>();
            services.AddScoped<ITruckTypeService, TruckTypeService>();
            services.AddScoped<ISewageServiceProviderService, SewageServiceProviderService>();
            services.AddScoped<IAwarenessContentService, AwarenessContentService>();
            services.AddScoped<IAnonymousDeviceService, AnonymousDeviceService>();
            //services.AddScoped<IDeviceTokenService, DeviceTokenService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAIAnalysisResultService, AIAnalysisResultService>();
            services.AddScoped<IReportAssignmentService, ReportAssignmentService>();
            services.AddScoped<IReportStatusHistoryService, ReportStatusHistoryService>();
            services.AddScoped<IAwarenessContentService, AwarenessContentService>();
            services.AddScoped<INotificationService, NotificationService>();
  

            return services;
        }
    }
}
