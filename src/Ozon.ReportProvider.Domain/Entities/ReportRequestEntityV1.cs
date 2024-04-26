namespace Ozon.ReportProvider.Domain.Entities;

public record ReportRequestEntityV1
{
    public long Id { get; init; }
    public Guid UserId { get; init; }
    public Guid GoodId { get; init; }
    public long LayoutId { get; init; }
    public DateTimeOffset StartOfPeriod { get; init; }
    public DateTimeOffset EndOfPeriod { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}