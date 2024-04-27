using Dapper;
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
        services.AddPostgres(configuration);
        services.AddRedisRepositories();


        return services;
    }

    private static IServiceCollection AddRedisRepositories(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfigurationRoot configuration
    )
    {
        services
            .Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)))
            .Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));

        services.AddMigrations();

        return services;
    }

    private static IServiceCollection AddPostgres(this IServiceCollection services, IConfigurationRoot configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var dataBaseOptions = configuration.Get<DatabaseOptions>()
                              ?? throw new ArgumentNullException(nameof(DatabaseOptions),
                                  "DataBaseOptions is not configured");

        services.AddNpgsqlDataSource(dataBaseOptions.ConnectionString);

        return services;
    }
}