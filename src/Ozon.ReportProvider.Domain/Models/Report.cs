using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Domain.Models;

public record Report
{
    public RequestId RequestId { get; init; }
    public decimal ConversionRatio { get; init; }
    public long SoldCount { get; init; }
}