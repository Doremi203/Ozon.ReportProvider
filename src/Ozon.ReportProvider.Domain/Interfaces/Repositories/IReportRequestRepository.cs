using Ozon.ReportProvider.Domain.Entities;

namespace Ozon.ReportProvider.Domain.Interfaces.Repositories;

public interface IReportRequestRepository
{
    Task<long[]> Add(ReportRequestEntityV1[] reportRequests, CancellationToken token);
    Task<ReportRequestEntityV1?> GetById(long id, CancellationToken token);
    Task<ReportRequestEntityV1[]> GetByUserId(Guid userId, CancellationToken token);
}