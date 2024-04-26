namespace Ozon.ReportProvider.Domain.Events;

public record ReportRequestEvent
{
    public required DateTimeOffset StartOfPeriod { get; init; }
    public required DateTimeOffset EndOfPeriod { get; init; }
    public required Guid GoodId { get; init; }
    public required long LayoutId { get; init; }
}