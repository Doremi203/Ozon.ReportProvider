namespace Ozon.ReportProvider.Bll.Infrastructure;

public interface IDateTimeOffsetProvider
{
    DateTimeOffset UtcNow { get; }
}