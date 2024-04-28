using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportService
{
    Task StoreReports(Report[] reports, CancellationToken token);
    Task<Report[]> GetReports(RequestReportModel[] models, CancellationToken token);
}  