using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportService
{
    Task<Report> GetRequestedReports(RequestReportModel[] models, CancellationToken token);
}  