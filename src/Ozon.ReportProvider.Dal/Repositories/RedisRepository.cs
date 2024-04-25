using Ozon.ReportProvider.Dal.Config;
using StackExchange.Redis;

namespace Ozon.ReportProvider.Dal.Repositories;

public abstract class RedisRepository(RedisOptions redisOptions)
{
    private static ConnectionMultiplexer? _connection;

    protected abstract string KeyPrefix { get; }
    
    protected virtual TimeSpan KeyTtl => TimeSpan.MaxValue;
    
    protected async Task<IDatabase> GetConnection()
    {
        _connection ??= await ConnectionMultiplexer.ConnectAsync(redisOptions.ConnectionString);
        
        return _connection.GetDatabase();
    }
    
    protected RedisKey GetKey(params object[] identifiers)
        => new ($"{KeyPrefix}:{string.Join(':', identifiers)}");
}