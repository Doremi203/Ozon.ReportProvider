using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ozon.ReportProvider.Dal.Extensions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;

namespace Ozon.ReportProvider.IntegrationTests.Fixtures;

public class DalTestFixture
{
    public IReportRepository ReportRepository { get; init; }

    public DalTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => { services.AddDal(configuration); })
            .Build();

        ClearDatabase(host);
        host.MigrateUp();

        var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        ReportRepository = serviceProvider.GetRequiredService<IReportRepository>();

        FluentAssertionOptions.UseDefaultPrecision();
    }

    private static void ClearDatabase(IHost host)
    {
        host.MigrateDown();
    }
}