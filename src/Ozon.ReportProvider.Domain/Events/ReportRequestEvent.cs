namespace Ozon.ReportProvider.Domain.Events;

public record ReportRequestEvent
{
    public required Guid RequestId { get; init; }
    public required Guid GoodId { get; init; }
    public required DateTimeOffset StartOfPeriod { get; init; }
    public required DateTimeOffset EndOfPeriod { get; init; }
}