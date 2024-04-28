namespace Ozon.ReportProvider.Api.Models;

public record GetReportModel
{
    public Guid RequestId { get; init; }
    public Guid GoodId { get; init; }
    public DateTimeOffset StartOfPeriod { get; init; }
    public DateTimeOffset EndOfPeriod { get; init; }
}