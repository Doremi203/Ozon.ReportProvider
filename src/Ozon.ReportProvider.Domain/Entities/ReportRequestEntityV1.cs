namespace Ozon.ReportProvider.Domain.Entities;

public record ReportRequestEntityV1
{
    public Guid RequestId { get; init; }
    public Guid GoodId { get; init; }
    public DateTimeOffset StartOfPeriod { get; init; }
    public DateTimeOffset EndOfPeriod { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}