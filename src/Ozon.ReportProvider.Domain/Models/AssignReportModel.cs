namespace Ozon.ReportProvider.Domain.Models;

public record AssignReportModel
{
    public long ReportId { get; init; }
    public Guid RequestId { get; init; }
}