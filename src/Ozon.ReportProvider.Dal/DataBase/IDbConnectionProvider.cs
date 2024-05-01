using System.Data.Common;

namespace Ozon.ReportProvider.Dal.DataBase;

public interface IDbConnectionProvider
{
    Task<DbConnection> OpenConnectionAsync(CancellationToken token);
}