namespace Ozon.ReportProvider.Api.Kafka.Contracts;

public record ReportRequestEventContract
{
    public required long RequestId { get; init; }
    public required long GoodId { get; init; }
    public required DateTimeOffset StartOfPeriod { get; init; }
    public required DateTimeOffset EndOfPeriod { get; init; }
}