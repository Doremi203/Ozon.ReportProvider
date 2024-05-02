using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IApiReportService
{
    Task<Report[]> GetReports(ApiGetReportModel[] models, CancellationToken token);
}