namespace Ozon.ReportProvider.Domain.Entities;

public record ReportEntityV1
{
    public long RequestId { get; init; }
    public decimal ConversionRatio { get; init; }
    public long SoldCount { get; init; }
}