using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportService
{
    Task RequestReports(RequestReportModel[] models);
    Task<ReportEvent[]> GetReports(Guid[] requestIds);
}  