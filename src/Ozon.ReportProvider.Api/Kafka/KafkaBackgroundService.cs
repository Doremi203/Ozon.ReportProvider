using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Kafka;
using Ozon.ReportProvider.Kafka.Config;

namespace Ozon.ReportProvider.Api.Kafka;

public class KafkaBackgroundService<TKey, TValue, THandler>(
    IOptions<KafkaSettings> settings,
    IServiceProvider serviceProvider,
    ILogger<KafkaBackgroundService<TKey, TValue, THandler>> logger,
    IHandler<TKey, TValue> handler,
    IDeserializer<TKey>? keyDeserializer,
    IDeserializer<TValue>? valueDeserializer
) : BackgroundService where THandler : IHandler<TKey, TValue>
{
    private readonly KafkaAsyncConsumer<TKey, TValue> _asyncConsumer = new(
        settings,
        serviceProvider.GetRequiredService<ILogger<KafkaAsyncConsumer<TKey, TValue>>>(),
        handler,
        keyDeserializer,
        valueDeserializer
    );

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _asyncConsumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _asyncConsumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occured");
        }
    }
}