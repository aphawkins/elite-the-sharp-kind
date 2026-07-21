// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Microsoft.Extensions.Logging;

namespace EliteSharpLib;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Failed to create {ShipType}: universe is full.")]
    internal static partial void FailedToCreateShip(ILogger logger, string shipType);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Failed to read commander file '{Path}'.")]
    internal static partial void FailedToLoadCommander(ILogger logger, string path, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Commander file '{Path}' failed validation.")]
    internal static partial void CommanderValidationFailed(ILogger logger, string path);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Failed to save commander file '{Path}'.")]
    internal static partial void FailedToSaveCommander(ILogger logger, string path, Exception ex);
}
