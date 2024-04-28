namespace Ozon.ReportProvider.Domain.Events;

public record ReportEvent
{
    public decimal ConversionRatio { get; init; }
    public long SoldCount { get; init; }
}