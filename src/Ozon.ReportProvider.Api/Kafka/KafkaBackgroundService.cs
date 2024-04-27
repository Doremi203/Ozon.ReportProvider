using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Kafka;
using Ozon.ReportProvider.Kafka.Config;
using Polly;

namespace Ozon.ReportProvider.Api.Kafka;

public class KafkaBackgroundService<TKey, TValue, THandler> : BackgroundService where THandler : IHandler<TKey, TValue>
{
    private readonly KafkaAsyncConsumer<TKey, TValue> _asyncConsumer;

    private readonly ILogger<KafkaBackgroundService<TKey, TValue, THandler>> _logger;

    public KafkaBackgroundService(IOptions<KafkaSettings> options,
        IServiceProvider serviceProvider,
        ILogger<KafkaBackgroundService<TKey, TValue, THandler>> logger,
        IHandler<TKey, TValue> handler,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer)
    {
        _logger = logger;
        
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = options.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = false
        };

        var consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();

        consumer.Subscribe(options.Value.Topic);
        
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                x =>
                {
                    var exponentDelay = Math.Pow(2, x);
                    return TimeSpan.FromSeconds(exponentDelay);
                },
                (exception, retry, _) =>
                {
                    _logger.LogError(exception, $"Error while handling message, retry number: {retry}");
                }
            );
        
        _asyncConsumer = new(
            options,
            serviceProvider.GetRequiredService<ILogger<KafkaAsyncConsumer<TKey, TValue>>>(),
            handler,
            consumer,
            policy
        );
    }

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
            _logger.LogError(ex, "Unhandled exception occured");
        }
    }
}