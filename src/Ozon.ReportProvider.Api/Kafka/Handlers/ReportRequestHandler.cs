using Confluent.Kafka;
using Ozon.ReportProvider.Domain.Events;
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
            var reportRequest = new ReportRequest
            {
                UserId = result.Message.Key,
                StartOfPeriod = reportRequestEvent.StartOfPeriod,
                EndOfPeriod = reportRequestEvent.EndOfPeriod,
                GoodId = reportRequestEvent.GoodId,
                LayoutId = reportRequestEvent.LayoutId
            };
            await reportRequestService.ProcessReportRequests([reportRequest], cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process message with offset: {result.Offset.Value}");
            throw;
        }
    }
}