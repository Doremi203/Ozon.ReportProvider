namespace Ozon.ReportProvider.Domain.Exceptions;

public class ReportNotReadyException : Exception
{
    public ReportNotReadyException()
    {
    }

    public ReportNotReadyException(string? message) : base(message)
    {
    }

    public ReportNotReadyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}