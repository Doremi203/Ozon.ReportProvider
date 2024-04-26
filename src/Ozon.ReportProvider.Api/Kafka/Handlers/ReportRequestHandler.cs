using Confluent.Kafka;
using Ozon.ReportProvider.Domain.Interfaces;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Kafka;

namespace Ozon.ReportProvider.Api.Kafka.Handlers;

public class ReportRequestHandler(
    ILogger<ReportRequestHandler> logger,
    IReportRequestService reportRequestService
    ) : IHandler<Guid, ReportRequestEvent>
{
    public async Task Handle(ConsumeResult<Guid, ReportRequestEvent> result, CancellationToken cancellationToken)
    {
        try
        {
            var reportRequestEvent = result.Message.Value;
            await reportRequestService.ProcessReportRequests([reportRequestEvent], cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process message with offset: {result.Offset.Value}");
            throw;
        }
    }
}