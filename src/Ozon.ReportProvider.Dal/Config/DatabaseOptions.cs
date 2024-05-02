namespace Ozon.ReportProvider.Dal.Config;

public record DatabaseOptions
{
    public required string UserId { get; init; }
    public required string Password { get; init; }
    public required string Host { get; init; }
    public required string Database { get; init; }
    public required string Options { get; init; }

    public string ConnectionString => $"""
                                       User ID={UserId};
                                       Password={Password};
                                       Host={Host};
                                       Database={Database};
                                       {Options}
                                       """;
}