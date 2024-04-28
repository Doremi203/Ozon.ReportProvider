using Mapster;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Utils.Providers;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportRequestService(
    IDateTimeOffsetProvider dateTimeOffsetProvider,
    IReportRequestRepository requestRepository
) : IReportRequestService
{
    public async Task StoreReportRequests(ReportRequestEvent[] reportRequestEvents, CancellationToken token)
    {
        var createdAt = dateTimeOffsetProvider.UtcNow;
        var reportRequests = reportRequestEvents.Select(e =>
            e.Adapt<ReportRequestEntityV1>() with { CreatedAt = createdAt }
        ).ToArray();

        await requestRepository.Add(reportRequests, token);
    }

    public Task<ReportRequestEntityV1[]> GetUncompletedReportRequests(int limit, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}