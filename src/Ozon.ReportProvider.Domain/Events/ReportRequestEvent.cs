using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Domain.Events;

public record ReportRequestEvent
{
    public required RequestId RequestId { get; init; }
    public required GoodId GoodId { get; init; }
    public required DateTimeOffset StartOfPeriod { get; init; }
    public required DateTimeOffset EndOfPeriod { get; init; }
}