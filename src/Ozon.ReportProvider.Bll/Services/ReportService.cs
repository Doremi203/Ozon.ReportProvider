using Mapster;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportService(
    IReportRepository reportRepository
) : IReportService
{
    public async Task StoreReports(Report[] reports, CancellationToken token)
    {
        await reportRepository.Add(reports.Adapt<ReportEntityV1[]>(), token);
    }

    public Task<Report[]> GetReports(RequestReportModel[] models, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}