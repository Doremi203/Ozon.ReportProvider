namespace Ozon.ReportProvider.Bll.Infrastructure;

public class DateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}