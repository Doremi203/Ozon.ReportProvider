using Confluent.Kafka;

namespace Ozon.ReportProvider.Kafka;

public interface IHandler<TKey, TValue>
{
    Task Handle(ConsumeResult<TKey, TValue> result, CancellationToken cancellationToken);
}