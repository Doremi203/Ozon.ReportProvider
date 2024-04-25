namespace Ozon.ReportProvider.Dal.Config;

public record DataBaseOptions
{
    public required string ConnectionString { get; init; }
}