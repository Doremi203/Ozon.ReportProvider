using Confluent.Kafka;
using Ozon.ReportProvider.Api.Kafka;
using Ozon.ReportProvider.Kafka;

namespace Ozon.ReportProvider.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaBackgroundService<TKey, TValue, THandler>(
        this IServiceCollection services,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer
    ) where THandler : IHandler<TKey, TValue>
    {
        services.AddSingleton(keyDeserializer);
        services.AddSingleton(valueDeserializer);
        services.AddHostedService<KafkaBackgroundService<TKey, TValue, THandler>>();

        return services;
    }
}