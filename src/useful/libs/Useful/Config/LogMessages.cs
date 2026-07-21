// 'Useful Libraries' - Andy Hawkins 2025.

using Microsoft.Extensions.Logging;

namespace Useful.Config;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Failed to read config file '{ConfigPath}'; using defaults.")]
    internal static partial void ConfigReadFailed(ILogger logger, string configPath);

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Config read failure detail for '{ConfigPath}'.")]
    internal static partial void ConfigReadFailedDetail(ILogger logger, string configPath, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Config file '{ConfigPath}' failed validation; using defaults.")]
    internal static partial void ConfigValidationFailed(ILogger logger, string configPath);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Failed to save config file '{ConfigPath}'.")]
    internal static partial void ConfigWriteFailed(ILogger logger, string configPath, Exception ex);
}
