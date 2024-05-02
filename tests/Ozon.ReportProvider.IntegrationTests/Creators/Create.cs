namespace Ozon.ReportProvider.IntegrationTests.Creators;

public class Create
{
    private static long _counter = DateTime.UtcNow.Ticks;
    
    private static readonly Random StaticRandom = new();
    
    public static long RandomId() => Interlocked.Increment(ref _counter);
    
    public static double RandomDouble() => StaticRandom.NextDouble();

    public static decimal RandomDecimal() => (decimal)StaticRandom.NextDouble();
    
    public static (DateTimeOffset start, DateTimeOffset end) RandomPeriod()
    {
        var start = DateTimeOffset.UtcNow.AddDays(-StaticRandom.Next(1, 100));
        var end = start.AddDays(StaticRandom.Next(1, 99));
        return (start, end);
    }
}