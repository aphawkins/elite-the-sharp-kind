// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;

namespace Useful.App;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Starting {title}")]
    internal static partial void StartingTitle(ILogger logger, string title);

    [LoggerMessage(EventId = 1, Level = LogLevel.Critical, Message = "Application terminated unexpectedly")]
    internal static partial void CriticalAppTerminated(ILogger logger, Exception ex);
}
