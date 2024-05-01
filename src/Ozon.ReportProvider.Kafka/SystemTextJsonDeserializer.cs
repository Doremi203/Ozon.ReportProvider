using System.Text.Json;
using Confluent.Kafka;

namespace Ozon.ReportProvider.Kafka;

public class SystemTextJsonDeserializer<T>(JsonSerializerOptions options) : IDeserializer<T>
{
    
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException(nameof(T),$"{nameof(T)} cannot be deserialized");

        return JsonSerializer.Deserialize<T>(data, options) 
               ?? throw new JsonException("Couldn't deserialize data correctly");
    }
}