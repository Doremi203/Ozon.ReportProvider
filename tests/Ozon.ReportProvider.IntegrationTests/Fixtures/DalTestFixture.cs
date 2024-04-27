using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ozon.ReportProvider.Dal.Extensions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;

namespace Ozon.ReportProvider.IntegrationTests.Fixtures;

public class DalTestFixture
{
    public IReportRequestRepository ReportRequestRepository { get; }
    
    public DalTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddDal(configuration);
            })
            .Build();
        
        ClearDatabase(host);
        
        var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        ReportRequestRepository = serviceProvider.GetRequiredService<IReportRequestRepository>();
    }

    private static void ClearDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationRunner.MigrateDown(0);
    }
}