using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Api.Services;

public interface IApiReportService
{
    Task<Report[]> GetReports(RequestReportModel[] models, CancellationToken token);
}