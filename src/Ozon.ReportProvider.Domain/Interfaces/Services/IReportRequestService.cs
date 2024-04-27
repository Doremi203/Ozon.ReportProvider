using Ozon.ReportProvider.Domain.Events;

namespace Ozon.ReportProvider.Domain.Interfaces.Services;

public interface IReportRequestService
{
    Task ProcessReportRequests(ReportRequestEvent[] reportRequests, CancellationToken cancellationToken);
}