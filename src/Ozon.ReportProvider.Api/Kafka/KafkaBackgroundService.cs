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
    private readonly KafkaConsumer<TKey, TValue> _consumer = new(
        settings,
        serviceProvider.GetRequiredService<ILogger<KafkaConsumer<TKey, TValue>>>(),
        handler,
        keyDeserializer,
        valueDeserializer
    );

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occured");
        }
    }
}