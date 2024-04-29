using Mapster;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportService(
    IReportRepository reportRepository
) : IReportService
{
    public async Task StoreReports(Report[] reports, CancellationToken token)
    {
        await reportRepository.Add(reports.Adapt<ReportEntityV1[]>(), token);
    }

    public async Task<Report[]> GetReports(RequestId[] requestIds, CancellationToken token)
    {
        var reportEntities = await reportRepository.GetReports(requestIds, token);

        return reportEntities.Adapt<Report[]>();
    }
}