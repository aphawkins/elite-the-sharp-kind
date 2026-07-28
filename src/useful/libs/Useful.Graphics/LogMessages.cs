// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;
using Useful.Assets;

namespace Useful.Graphics;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Warning,
        Message = "Asset colour budget exceeded for the {Tier} tier: {ColourCount} distinct opaque colours against a cap of {Cap}.")]
    internal static partial void AssetColourBudgetExceeded(ILogger logger, SystemTier tier, int colourCount, int cap);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Asset '{Asset}' contributes {ColourCount} distinct opaque colours.")]
    internal static partial void AssetColourCount(ILogger logger, string asset, int colourCount);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "{PixelCount} asset pixels have partial alpha; the renderer treats transparency as all-or-nothing.")]
    internal static partial void AssetPartialAlpha(ILogger logger, int pixelCount);
}
