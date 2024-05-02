using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.ReportProvider.Kafka.Common;
using Ozon.ReportProvider.Kafka.Config;
using Polly;

namespace Ozon.ReportProvider.Kafka;

public sealed class KafkaAsyncConsumer<TKey, TValue> : IDisposable
{
    private readonly TimeSpan _bufferDelay;
    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly int _channelCapacity;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IHandler<TKey, TValue> _handler;
    private readonly ILogger<KafkaAsyncConsumer<TKey, TValue>> _logger;
    private readonly AsyncPolicy _policy;

    public KafkaAsyncConsumer(
        IOptions<KafkaOptions> options,
        ILogger<KafkaAsyncConsumer<TKey, TValue>> logger,
        IHandler<TKey, TValue> handler,
        IConsumer<TKey, TValue> consumer,
        AsyncPolicy policy
    )
    {
        _handler = handler;
        _logger = logger;
        _consumer = consumer;
        _channelCapacity = options.Value.BatchMaxSize;
        _bufferDelay = TimeSpan.FromSeconds(options.Value.BatchDelay);
        _policy = policy;

        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(_channelCapacity)
            {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public void Dispose()
    {
        _consumer.Close();
        _channel.Writer.TryComplete();
    }

    public Task Consume(CancellationToken token)
    {
        var handle = HandleCore(token);
        var consume = ConsumeCore(token);

        return Task.WhenAll(handle, consume);
    }

    private async Task HandleCore(CancellationToken token)
    {
        try
        {
            await foreach (var consumeResults in _channel.Reader
                               .ReadAllAsync(token)
                               .Buffer(_channelCapacity, _bufferDelay)
                               .WithCancellation(token))
                await ProcessWithRetry(consumeResults, token);
        }
        catch (Exception)
        {
            _channel.Writer.TryComplete();
            throw;
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

    private async Task ProcessWithRetry(IReadOnlyList<ConsumeResult<TKey, TValue>> consumeResults,
        CancellationToken token)
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
}