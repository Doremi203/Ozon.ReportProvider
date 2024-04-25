using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Kafka.Config;
using Polly;
using Polly.Retry;

namespace Ozon.ReportProvider.Kafka;

public sealed class KafkaConsumer<TKey, TValue> : IDisposable
{
    private readonly IHandler<TKey, TValue> _handler;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly ILogger<KafkaConsumer<TKey, TValue>> _logger;
    private readonly AsyncRetryPolicy _policy;

    public KafkaConsumer(
        IOptions<KafkaSettings> options,
        ILogger<KafkaConsumer<TKey, TValue>> logger,
        IHandler<TKey, TValue> handler,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer
    )
    {
        _handler = handler;
        _logger = logger;
        
        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                x =>
                {
                    var exponentDelay = Math.Pow(2, x);
                    return TimeSpan.FromSeconds(exponentDelay);
                });
        
        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(options.Value.ChannelCapacity)
            {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });
        
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = options.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = false
        };

        _consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();
        
        _consumer.Subscribe(options.Value.Topic);
    }

    public Task Consume(CancellationToken token)
    {
        var handle = HandleCore(token);
        var consume = ConsumeCore(token);

        return Task.WhenAll(handle, consume);
    }
    
    private async Task HandleCore(CancellationToken token)
    {
        await foreach(var consumeResult in _channel.Reader.ReadAllAsync(token))
        {
            await _policy.ExecuteAsync(async () =>
            {
                try
                {
                    await _handler.Handle(consumeResult, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while handling message, retrying...");
                    throw;
                }
                
                _consumer.StoreOffset(consumeResult);
            });
        }
    }

    private async Task ConsumeCore(CancellationToken token)
    {
        await Task.Yield();

        _logger.LogInformation("Start consuming");
        while (_consumer.Consume(token) is { } result)
        {
            await _channel.Writer.WriteAsync(result, token);
            _logger.LogTrace(
                "{Partition}:{Offset}:WriteToChannel",
                result.Partition.Value,
                result.Offset.Value);
        }

        _channel.Writer.Complete();
    }

    public void Dispose()
    {
        _consumer.Close();
        
    }
}