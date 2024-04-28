namespace Ozon.ReportProvider.Api.Config;

public record ReportServiceSettings
{
    public required string ReportServiceUrl { get; init; }
    public required int PollingInterval { get; init; }
    public required int MaxBatchSize { get; init; }
}