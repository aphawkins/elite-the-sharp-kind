// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Microsoft.Extensions.Logging;

namespace StuntCarRacer;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Starting {title}")]
    internal static partial void StartingTitle(ILogger logger, string title);

    [LoggerMessage(EventId = 1, Level = LogLevel.Critical, Message = "Application terminated unexpectedly")]
    internal static partial void CriticalAppTerminated(ILogger logger, Exception ex);
}
