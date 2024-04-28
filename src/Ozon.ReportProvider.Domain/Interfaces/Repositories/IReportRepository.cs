using Ozon.ReportProvider.Domain.Entities;

namespace Ozon.ReportProvider.Domain.Interfaces.Repositories;

public interface IReportRepository
{
    Task Add(ReportEntityV1[] reports, CancellationToken token);
    Task<ReportEntityV1[]> GetUncompletedReports(int limit, CancellationToken token);
}