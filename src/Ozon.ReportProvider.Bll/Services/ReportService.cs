using System.Text.Json;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Exceptions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportService(
    IReportRepository reportRepository,
    IDistributedCache distributedCache
) : IReportService
{
    public async Task StoreReports(Report[] reports, CancellationToken token)
    {
        await reportRepository.Add(reports.Adapt<ReportEntityV1[]>(), token);
    }

    public async Task<Report> GetReport(RequestId requestId, CancellationToken token)
    {
        var cacheKey = $"reports:{requestId.Value}";
        var cachedReport = await distributedCache.GetStringAsync(cacheKey, token);
        Report? report;
        if (!string.IsNullOrEmpty(cachedReport))
            report = JsonSerializer.Deserialize<Report?>(cachedReport);
        else
        {
            report = (await reportRepository.GetReport(requestId, token)).Adapt<Report?>();
            await CacheReport(cacheKey, report, token);
        }

        if (report is null)
            throw new ReportNotReadyException("Report is not ready yet. Please try again later.");

        return report;
    }

    public async Task<ReportRequestEvent[]> GetUncompleteReportRequests(ReportRequestEvent[] requests, CancellationToken token)
    {
        var requestIds = requests.Select(r => r.RequestId).ToArray();
        var completedRequestIds = await reportRepository.GetCompletedRequestIds(requestIds, token);

        return requests.Where(r => !completedRequestIds.Contains(r.RequestId)).ToArray();
    }

    private Task CacheReport(string cacheKey, Report? report, CancellationToken token)
    {
        return distributedCache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(report),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            token
        );
    }
}