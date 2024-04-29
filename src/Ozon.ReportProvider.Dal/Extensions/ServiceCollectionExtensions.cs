using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.ReportProvider.Dal.Config;
using Ozon.ReportProvider.Dal.Repositories;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;

namespace Ozon.ReportProvider.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDal(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddInfrastructure(configuration);
        services.AddPostgres(configuration);
        services.AddPostgresRepositories();
        services.AddRedisRepositories();


        return services;
    }

    private static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddTransient<IReportRepository, ReportRepository>();

        return services;
    }

    private static IServiceCollection AddRedisRepositories(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)))
            .Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));

        services.AddMigrations();

        return services;
    }

    private static IServiceCollection AddPostgres(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var dataBaseOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();

        if (dataBaseOptions is not null)
        {
            services.AddNpgsqlDataSource(
                dataBaseOptions.ConnectionString,
                builder => { builder.MapComposite<ReportEntityV1>("reports_v1", builder.DefaultNameTranslator); });
        }
        
        return services;
    }
}