namespace Ozon.ReportProvider.Domain.Models;

public record RequestReportModel
{
    public Guid RequestId { get; init; }
    public Guid GoodId { get; init; }
    public DateTimeOffset StartOfPeriod { get; init; }
    public DateTimeOffset EndOfPeriod { get; init; }
}