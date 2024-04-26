using Confluent.Kafka;
using Ozon.ReportProvider.Api.Kafka;
using Ozon.ReportProvider.Kafka;

namespace Ozon.ReportProvider.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaBackgroundService<TKey, TValue, THandler>(
        this IServiceCollection services
    ) where THandler : IHandler<TKey, TValue>
    {
        services.AddSingleton<IDeserializer<TKey>, SystemTextJsonDeserializer<TKey>>();
        services.AddSingleton<IDeserializer<TValue>, SystemTextJsonDeserializer<TValue>>();
        services.AddHostedService<KafkaBackgroundService<TKey, TValue, THandler>>();

        return services;
    }
}