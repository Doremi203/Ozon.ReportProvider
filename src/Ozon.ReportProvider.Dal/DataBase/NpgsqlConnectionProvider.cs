using System.Data.Common;
using Npgsql;

namespace Ozon.ReportProvider.Dal.DataBase;

public class NpgsqlConnectionProvider(
    NpgsqlDataSource dataSource
) : IDbConnectionProvider
{
    public async Task<DbConnection> OpenConnectionAsync(CancellationToken token)
    {
        return await dataSource.OpenConnectionAsync(token);
    }
}