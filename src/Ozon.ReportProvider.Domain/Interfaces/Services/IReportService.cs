using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportService
{
    Task StoreReports(Report[] reports, CancellationToken token);
    Task<Report> GetReport(RequestId requestId, CancellationToken token);
    Task<Report[]> GetReports(RequestId[] requestIds, CancellationToken token);
}  