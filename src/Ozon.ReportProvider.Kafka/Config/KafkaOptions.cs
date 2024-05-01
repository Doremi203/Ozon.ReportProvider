namespace Ozon.ReportProvider.Kafka.Config;

public record KafkaOptions
{
    public required string BootstrapServers { get; init; }
    public required string Topic { get; init; }
    public required string GroupId { get; init; }
    public required int BatchMaxSize { get; init; }
    public required int BatchDelay { get; init; }
}