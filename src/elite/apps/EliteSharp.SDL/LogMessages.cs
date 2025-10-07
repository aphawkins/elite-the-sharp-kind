// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System;
using Microsoft.Extensions.Logging;

namespace EliteSharp.SDL;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Starting {title}")]
    internal static partial void StartingTitle(ILogger logger, string title);

    [LoggerMessage(EventId = 1, Level = LogLevel.Critical, Message = "Application terminated unexpectedly")]
    internal static partial void CriticalAppTerminated(ILogger logger, Exception ex);
}
