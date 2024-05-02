using Confluent.Kafka;

namespace Ozon.ReportProvider.Kafka;

public interface IHandler<TKey, TValue>
{
    Task Handle(IReadOnlyList<ConsumeResult<TKey, TValue>> messages, CancellationToken token);
}