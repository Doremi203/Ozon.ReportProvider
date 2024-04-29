using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Domain.Interfaces.Repositories;

public interface IReportRepository
{
    Task Add(ReportEntityV1[] reports, CancellationToken token);
    Task<ReportEntityV1[]> GetReports(RequestId[] requestIds, CancellationToken token);
}