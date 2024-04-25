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
        throw new NotImplementedException();
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfigurationRoot configuration
    )
    {
        services
            .Configure<DataBaseOptions>(configuration.GetSection(nameof(DataBaseOptions)))
            .Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));

        return services;
    }
    
    private static IServiceCollection AddPostgres(this IServiceCollection services, IConfigurationRoot configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var dataBaseOptions = configuration.Get<DataBaseOptions>()
            ?? throw new ArgumentNullException(nameof(DataBaseOptions), "DataBaseOptions is not configured");
        
        services.AddNpgsqlDataSource(dataBaseOptions.ConnectionString);

        return services;
    }
}