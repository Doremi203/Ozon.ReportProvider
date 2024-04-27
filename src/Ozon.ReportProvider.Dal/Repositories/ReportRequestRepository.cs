using Dapper;
using Npgsql;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;

namespace Ozon.ReportProvider.Dal.Repositories;

public class ReportRequestRepository(
    NpgsqlDataSource dataSource
) : IReportRequestRepository
{
    public async Task<long[]> Add(ReportRequestEntityV1[] reportRequests, CancellationToken token)
    {
        const string sql = @"
insert into report_requests (id, user_id, good_id, layout_id, start_of_period, end_of_period, created_at)
select id, user_id, good_id, layout_id, start_of_period, end_of_period, created_at
from unnest(@ReportRequests)
returning id
";
        await using var connection = await dataSource.OpenConnectionAsync(token);

        var ids = await connection.QueryAsync<long>(
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
        
        return ids.ToArray();
    }

    public async Task<ReportRequestEntityV1[]> GetByUserId(Guid userId, CancellationToken token)
    {
        const string sql = @"
select id
     , user_id
     , good_id
     , layout_id
     , start_of_period
     , end_of_period
     , created_at
from report_requests
where user_id = @UserId
";
        await using var connection = await dataSource.OpenConnectionAsync(token);

        return (await connection.QueryAsync<ReportRequestEntityV1>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId
                },
                cancellationToken: token,
                commandTimeout: Postgres.DefaultTimeout
            )
        )).ToArray();
    }
}