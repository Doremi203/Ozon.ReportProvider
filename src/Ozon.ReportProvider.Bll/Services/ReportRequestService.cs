using Mapster;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Utils.Providers;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportRequestService(
    IDateTimeOffsetProvider dateTimeOffsetProvider,
    IReportRequestRepository requestRepository
) : IReportRequestService
{
    public async Task ProcessReportRequests(ReportRequestEvent[] reportRequestEvents, CancellationToken token)
    {
        var createdAt = dateTimeOffsetProvider.UtcNow;
        var reportRequests = reportRequestEvents.Select(e =>
            e.Adapt<ReportRequestEntityV1>() with { CreatedAt = createdAt }
        ).ToArray();

        await requestRepository.Add(reportRequests, token);
    }
}