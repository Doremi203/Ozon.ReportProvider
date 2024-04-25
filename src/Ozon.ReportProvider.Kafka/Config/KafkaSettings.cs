namespace Ozon.ReportProvider.Kafka.Config;

public record KafkaSettings
{
    public required string BootstrapServers { get; init; }
    public required string Topic { get; init; }
    public required string GroupId { get; init; }
    public required int ChannelCapacity { get; init; }
}