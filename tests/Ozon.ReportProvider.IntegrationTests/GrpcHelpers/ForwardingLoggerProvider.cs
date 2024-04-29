using Microsoft.Extensions.Logging;

namespace Ozon.ReportProvider.IntegrationTests.GrpcHelpers;

internal class ForwardingLoggerProvider(
    LogMessage logAction
) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new ForwardingLogger(categoryName, logAction);
    }

    public void Dispose()
    {
    }

    internal class ForwardingLogger(
        string categoryName,
        LogMessage logAction
    ) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            logAction(logLevel, categoryName, eventId, formatter(state, exception), exception);
        }
    }
}