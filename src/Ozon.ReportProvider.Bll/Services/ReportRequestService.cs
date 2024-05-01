using Mapster;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportRequestService(
    IApiReportService apiReportService,
    IReportService reportService
) : IReportRequestService
{
    public async Task ProcessReportRequests(ReportRequestEvent[] reportRequestEvents, CancellationToken token)
    {
        var uncompleteReportRequests = await reportService.GetUncompleteReportRequests(reportRequestEvents, token);
        if (uncompleteReportRequests.Length == 0)
            return;
        var reports = 
            await apiReportService.GetReports(uncompleteReportRequests.Adapt<ApiGetReportModel[]>(), token);
        
        await reportService.StoreReports(reports, token);
    }
}