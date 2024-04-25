using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.ReportProvider.Dal.Config;

namespace Ozon.ReportProvider.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDal(
        this IServiceCollection services,
        IConfigurationRoot configuration
    )
    {
        services.AddInfrastructure(configuration);
        services.AddRedisRepositories();

        return services;
    }

    private static IServiceCollection AddRedisRepositories(this IServiceCollection services)
    {
        throw new NotImplementedException();
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfigurationRoot configuration
    )
    {
        services
            .Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));

        return services;
    }
}