using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ozon.ReportProvider.Dal.Extensions;

public static class HostExtensions
{
    public static IHost MigrateUp(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        return app;
    }
    
    public static IHost MigrateDown(this IHost app, int version = 0)
    {
        using var scope = app.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
        return app;
    }
}