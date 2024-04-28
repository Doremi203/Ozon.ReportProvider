using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Events;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportRequestService
{
    Task ProcessReportRequests(ReportRequestEvent[] reportRequestEvents, CancellationToken token);
}