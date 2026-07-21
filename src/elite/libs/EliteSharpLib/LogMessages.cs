// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Microsoft.Extensions.Logging;

namespace EliteSharpLib;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Failed to create {ShipType}: universe is full.")]
    internal static partial void FailedToCreateShip(ILogger logger, string shipType);
}
