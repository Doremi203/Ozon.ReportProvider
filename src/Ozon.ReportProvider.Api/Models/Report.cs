namespace Ozon.ReportProvider.Api.Models;

public record Report
{
    public Guid RequestId { get; init; }
    public decimal ConversionRatio { get; init; }
    public long SoldCount { get; init; }
}