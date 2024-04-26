using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Domain.Interfaces;

public interface IReportRequestService
{
    Task ProcessReportRequests(ReportRequestEvent[] reportRequestEvents);
}