using Dapper;
using Npgsql;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Dal.Repositories;

public class ReportRepository(
    NpgsqlDataSource dataSource
) : IReportRepository
{
    public async Task Add(ReportEntityV1[] reports, CancellationToken token)
    {
        const string sql = @"
insert into reports (request_id, conversion_ratio, sold_count)
select request_id, conversion_ratio, sold_count
from unnest(@Reports)
";
        await using var connection = await dataSource.OpenConnectionAsync(token);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Reports = reports
                },
                cancellationToken: token,
                commandTimeout: Postgres.DefaultTimeout
            )
        );
    }

    public async Task<ReportEntityV1[]> GetReports(RequestId[] requestIds, CancellationToken token)
    {
        const string sql = @"
select request_id
     , conversion_ratio
     , sold_count
from reports
where request_id = any(@RequestIds)
";
        await using var connection = await dataSource.OpenConnectionAsync(token);

        return (await connection.QueryAsync<ReportEntityV1>(
            new CommandDefinition(
                sql,
                new
                {
                    RequestIds = requestIds.Select(x => x.Value).ToArray()
                },
                cancellationToken: token,
                commandTimeout: Postgres.DefaultTimeout
            )
        )).ToArray();
    }
}