using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Kafka.Common;
using Ozon.ReportProvider.Kafka.Config;
using Polly;
using Polly.Retry;

namespace Ozon.ReportProvider.Kafka;

public sealed class KafkaAsyncConsumer<TKey, TValue> : IDisposable
{
    private readonly IHandler<TKey, TValue> _handler;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly ILogger<KafkaAsyncConsumer<TKey, TValue>> _logger;
    private readonly AsyncRetryPolicy _policy;
    private readonly int _channelCapacity;
    private readonly TimeSpan _bufferDelay;

    public KafkaAsyncConsumer(
        IOptions<KafkaSettings> options,
        ILogger<KafkaAsyncConsumer<TKey, TValue>> logger,
        IHandler<TKey, TValue> handler,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer
    )
    {
        _handler = handler;
        _logger = logger;
        _channelCapacity = options.Value.ChannelCapacity;
        _bufferDelay = TimeSpan.FromSeconds(options.Value.BufferDelay);

        _policy = Policy
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

        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(_channelCapacity)
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
        await foreach (var consumeResults in _channel.Reader
                           .ReadAllAsync(token)
                           .Buffer(_channelCapacity, _bufferDelay)
                           .WithCancellation(token))
        {
            try
            {
                await ProcessWithRetry(consumeResults, token);
            }
            finally
            {
                _channel.Writer.Complete();
            }
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

    private async Task ProcessWithRetry(IReadOnlyList<ConsumeResult<TKey, TValue>> consumeResults, CancellationToken token)
    {
        await _policy.ExecuteAsync(async () =>
        {
            await _handler.Handle(consumeResults, token);
            
            var partitionLastOffsets = consumeResults
                .GroupBy(
                    r => r.Partition.Value,
                    (_, f) => f.MaxBy(p => p.Offset.Value));

            foreach (var partitionLastOffset in partitionLastOffsets)
                _consumer.StoreOffset(partitionLastOffset);
        });
    }

    public void Dispose()
    {
        _consumer.Close();
        _channel.Writer.Complete();
    }
}