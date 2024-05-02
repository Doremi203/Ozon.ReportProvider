using Microsoft.Extensions.DependencyInjection;
using Ozon.ReportProvider.Bll.Services;
using Ozon.ReportProvider.Domain.Interfaces.Services;

namespace Ozon.ReportProvider.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        services.AddTransient<IApiReportService, ApiReportServiceFake>();
        services.AddTransient<IReportService, ReportService>();
        services.AddSingleton<IReportRequestService, ReportRequestService>();

        return services;
    }
}