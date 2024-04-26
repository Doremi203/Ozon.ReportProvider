namespace Ozon.ReportProvider.Domain.Models;

public record ReportRequest
{
    public required Guid UserId { get; init; }
    public required DateTimeOffset StartOfPeriod { get; init; }
    public required DateTimeOffset EndOfPeriod { get; init; }
    public required Guid GoodId { get; init; }
    public required long LayoutId { get; init; }
}