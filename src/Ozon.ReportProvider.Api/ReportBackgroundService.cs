using Mapster;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Api.Config;
using Ozon.ReportProvider.Api.Models;
using Ozon.ReportProvider.Api.Services;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Report = Ozon.ReportProvider.Domain.Models.Report;

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
                reportRequests.Adapt<GetReportModel[]>(), stoppingToken);
            
            await reportService.StoreReports(reports.Adapt<Report[]>(), stoppingToken);
            
            await Task.Delay(pollingInterval, stoppingToken);
        }
    }
}