using Dapper;
using Npgsql;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;

namespace Ozon.ReportProvider.Dal.Repositories;

public class ReportRequestRepository(
    NpgsqlDataSource dataSource
) : IReportRequestRepository
{
    public async Task Add(ReportRequestEntityV1[] reportRequests, CancellationToken token)
    {
        const string sql = @"
insert into report_requests (request_id, good_id, start_of_period, end_of_period, created_at)
select request_id, good_id, start_of_period, end_of_period, created_at
from unnest(@ReportRequests)
";
        await using var connection = await dataSource.OpenConnectionAsync(token);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    ReportRequests = reportRequests
                },
                cancellationToken: token,
                commandTimeout: Postgres.DefaultTimeout
            )
        );
    }

    public async Task<ReportRequestEntityV1[]> GetByIds(Guid[] ids, CancellationToken token)
    {
        const string sql = @"
select request_id
     , good_id
     , start_of_period
     , end_of_period
     , created_at
from report_requests
where request_id = any(@Ids)
";
        await using var connection = await dataSource.OpenConnectionAsync(token);

        return (await connection.QueryAsync<ReportRequestEntityV1>(
            new CommandDefinition(
                sql,
                new
                {
                    Ids = ids
                },
                cancellationToken: token,
                commandTimeout: Postgres.DefaultTimeout
            )
        )).ToArray();
    }
}