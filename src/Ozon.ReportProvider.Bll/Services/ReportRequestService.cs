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
        var reports = await apiReportService.GetReports(reportRequestEvents.Adapt<GetReportModel[]>(), token);
        
        await reportService.StoreReports(reports, token);
    }
}