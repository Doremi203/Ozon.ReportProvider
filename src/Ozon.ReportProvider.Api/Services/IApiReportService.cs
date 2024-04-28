using Ozon.ReportProvider.Api.Models;

namespace Ozon.ReportProvider.Api.Services;

public interface IApiReportService
{
    Task<Report[]> GetReports(GetReportModel[] models, CancellationToken token);
}