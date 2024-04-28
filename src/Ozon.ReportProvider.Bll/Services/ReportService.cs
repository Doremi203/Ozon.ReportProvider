using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Bll.Services;

public class ReportService : IReportService
{
    public Task StoreReports(Report[] reports, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Report[]> GetReports(RequestReportModel[] models, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}