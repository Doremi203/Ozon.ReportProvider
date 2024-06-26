using Confluent.Kafka;
using Mapster;
using Ozon.ReportProvider.Api.Kafka.Contracts;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Kafka;

namespace Ozon.ReportProvider.Api.Kafka.Handlers;

public class ReportRequestHandler(
    ILogger<ReportRequestHandler> logger,
    IReportRequestService reportRequestService
) : IHandler<Null, ReportRequestEventContract>
{
    public async Task Handle(IReadOnlyList<ConsumeResult<Null, ReportRequestEventContract>> messages,
        CancellationToken cancellationToken)
    {
        try
        {
            var reportRequestEvents = messages.Select(x => x.Message.Value).ToArray();
            await reportRequestService.ProcessReportRequests(reportRequestEvents.Adapt<ReportRequestEvent[]>(),
                cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process messages with starting offset: {messages[0].Offset.Value}");
            throw;
        }
    }
}