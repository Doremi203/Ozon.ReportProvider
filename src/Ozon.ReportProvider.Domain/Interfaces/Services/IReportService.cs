using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportService
{
    Task StoreReports(Report[] reports, CancellationToken token);
    Task<Report> GetReport(RequestId requestId, CancellationToken token);
    Task<ReportRequestEvent[]> GetUncompleteReportRequests(ReportRequestEvent[] requests, CancellationToken token);
}  