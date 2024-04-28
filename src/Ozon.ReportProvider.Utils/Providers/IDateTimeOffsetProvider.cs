namespace Ozon.ReportProvider.Utils.Providers;

public interface IDateTimeOffsetProvider
{
    DateTimeOffset UtcNow { get; }
}