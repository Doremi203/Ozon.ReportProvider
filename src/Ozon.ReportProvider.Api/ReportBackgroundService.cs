using Mapster;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Api.Config;
using Ozon.ReportProvider.Api.Services;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Api;

public class ReportBackgroundService(
    IOptionsMonitor<ReportServiceSettings> settings,
    IReportService reportService,
    IApiReportService apiReportService,
    IReportRequestService reportRequestService
    ) : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var pollingInterval = settings.CurrentValue.PollingInterval;
            var maxBatchSize = settings.CurrentValue.MaxBatchSize;
            
            var reportRequests = 
                await reportRequestService.GetUncompletedReportRequests(maxBatchSize, stoppingToken);
            var reports = await apiReportService.GetReports(
                reportRequests.Adapt<RequestReportModel[]>(), stoppingToken);
            
            await reportService.StoreReports(reports, stoppingToken);
            
            await Task.Delay(pollingInterval, stoppingToken);
        }
    }
}