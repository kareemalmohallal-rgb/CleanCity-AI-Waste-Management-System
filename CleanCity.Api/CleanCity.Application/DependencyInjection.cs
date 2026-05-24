using CleanCity.Application.Interfaces.Repositories;
using CleanCity.Application.Interfaces.Services;
using CleanCity.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITruckService, TruckService>();
            services.AddScoped<ITruckTypeService, TruckTypeService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<ISewageServiceProviderService, SewageServiceProviderService>();
            services.AddScoped<IAwarenessContentService, AwarenessContentService>();
            services.AddScoped<ITruckTypeService, TruckTypeService>();
            services.AddScoped<ITruckService, TruckService>();
            services.AddScoped<IReportAssignmentService, ReportAssignmentService>();
            services.AddScoped<IAnonymousDeviceService, AnonymousDeviceService>();
            services.AddScoped<IAIAnalysisResultService, AIAnalysisResultService>();
            services.AddScoped<IReportStatusHistoryService, ReportStatusHistoryService>();
            services.AddScoped<IAwarenessContentService, AwarenessContentService>();
            services.AddScoped<INotificationService, NotificationService>();
            // لاحقًا:
            // services.AddScoped<IAreaService, AreaService>();
            // services.AddScoped<IDriverService, DriverService>();
            // services.AddScoped<ITruckService, TruckService>();
            // ...

            return services;
        }
    }
}
