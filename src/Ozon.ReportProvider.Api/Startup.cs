using System.Text.Json;
using Confluent.Kafka;
using FluentValidation;
using Ozon.ReportProvider.Api.Config;
using Ozon.ReportProvider.Api.Extensions;
using Ozon.ReportProvider.Api.Interceptors;
using Ozon.ReportProvider.Api.Kafka.Contracts;
using Ozon.ReportProvider.Api.Kafka.Handlers;
using Ozon.ReportProvider.Api.Services;
using Ozon.ReportProvider.Bll.Extensions;
using Ozon.ReportProvider.Dal.Config;
using Ozon.ReportProvider.Dal.Extensions;
using Ozon.ReportProvider.Kafka;
using Ozon.ReportProvider.Kafka.Extensions;

namespace Ozon.ReportProvider.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafkaOptions(configuration);
        services.AddSingleton<ReportRequestHandler>();
        services.AddKafkaBackgroundService<Null, ReportRequestEventContract, ReportRequestHandler>(
            Deserializers.Null,
            new SystemTextJsonDeserializer<ReportRequestEventContract>(new JsonSerializerOptions()));

        services.AddGrpc(options => { options.Interceptors.Add<ValidationInterceptor>(); });
        services.AddGrpcReflection();
        services.AddValidatorsFromAssemblyContaining<Startup>();

        MapsterConfig.ConfigureApiMapping();
        Bll.Config.MapsterConfig.ConfigureDomainMapping();

        services
            .AddBllServices()
            .AddDal(configuration);
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =
                configuration.GetSection(nameof(RedisOptions)).Get<RedisOptions>()?.ConnectionString;
        });
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