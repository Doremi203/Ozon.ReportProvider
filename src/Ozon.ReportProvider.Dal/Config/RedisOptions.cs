namespace Ozon.ReportProvider.Dal.Config;

public record RedisOptions
{
    public required string ConnectionString { get; set; }
}