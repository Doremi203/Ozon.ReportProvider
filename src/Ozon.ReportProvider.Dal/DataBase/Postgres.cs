using System.Transactions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Dal.Config;

namespace Ozon.ReportProvider.Dal.DataBase;

public static class Postgres
{
    public const int DefaultTimeout = 10;

    public static TransactionScope CreateTransactionScope(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = TimeSpan.FromSeconds(DefaultTimeout)
            });
    }

    public static void AddMigrations(this IServiceCollection services)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(
                builder => builder
                    .AddPostgres()
                    .WithGlobalConnectionString(s =>
                    {
                        var cfg = s.GetRequiredService<IOptions<DatabaseOptions>>();
                        return cfg.Value.ConnectionString;
                    })
                    .ScanIn(typeof(Postgres).Assembly).For.Migrations()
            )
            .AddLogging(builder => builder.AddFluentMigratorConsole());
    }
}