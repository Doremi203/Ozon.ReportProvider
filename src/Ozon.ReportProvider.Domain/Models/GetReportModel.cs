namespace Ozon.ReportProvider.Domain.Models;

public record GetReportModel
{
    public required Guid RequestId { get; init; }
    public required Guid GoodId { get; init; }
    public required DateTimeOffset StartOfPeriod { get; init; }
    public required DateTimeOffset EndOfPeriod { get; init; }
}