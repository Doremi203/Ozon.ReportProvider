using Ozon.ReportProvider.Domain.Entities;

namespace Ozon.ReportProvider.Domain.Interfaces.Repositories;

public interface IReportRequestRepository
{
    Task Add(ReportRequestEntityV1[] reportRequests, CancellationToken token);
    Task<ReportRequestEntityV1[]> GetByUserId(Guid userId, CancellationToken token);
}