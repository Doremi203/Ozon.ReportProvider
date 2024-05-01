using System.Text.Json;
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
        services.AddSingleton<IDeserializer<TKey>, SystemTextJsonDeserializer<TKey>>(_ => new SystemTextJsonDeserializer<TKey>(new JsonSerializerOptions()));
        services.AddSingleton<IDeserializer<TValue>, SystemTextJsonDeserializer<TValue>>(_ => new SystemTextJsonDeserializer<TValue>(new JsonSerializerOptions()));
        services.AddHostedService<KafkaBackgroundService<TKey, TValue, THandler>>();

        return services;
    }
}