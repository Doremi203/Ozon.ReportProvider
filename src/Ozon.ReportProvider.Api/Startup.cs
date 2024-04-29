using Ozon.ReportProvider.Api.Config;
using Ozon.ReportProvider.Api.Services;
using Ozon.ReportProvider.Bll.Extensions;
using Ozon.ReportProvider.Dal.Extensions;

namespace Ozon.ReportProvider.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddGrpcReflection();

        MapsterConfig.Configure();
        services
            .AddBllServices()
            .AddDal(configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<ReportsGrpcService>();
            endpoints.MapGrpcReflectionService();
        });
    }
}