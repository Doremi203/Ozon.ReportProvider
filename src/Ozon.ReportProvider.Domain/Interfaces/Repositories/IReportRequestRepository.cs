using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Domain.Interfaces.Repositories;

public interface IReportRequestRepository
{
    Task Add(ReportRequestEntityV1[] reportRequests, CancellationToken token);
    Task AssignReportToRequest(AssignReportModel model, CancellationToken token);
    Task<ReportRequestEntityV1[]> GetByIds(Guid[] ids, CancellationToken token);
}