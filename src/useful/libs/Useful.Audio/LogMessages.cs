// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;

namespace Useful.Audio;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Warning,
        Message = "No sample is registered for effect '{EffectType}'; nothing played.")]
    internal static partial void MissingSfxSample(ILogger logger, string effectType);
}
