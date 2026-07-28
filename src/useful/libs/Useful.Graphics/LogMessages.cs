// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;

namespace Useful.Graphics;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Asset '{Asset}' contributes {ColourCount} distinct opaque colours.")]
    internal static partial void AssetColourCount(ILogger logger, string asset, int colourCount);
}
