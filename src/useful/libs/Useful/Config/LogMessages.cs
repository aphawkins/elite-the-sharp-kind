// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;

namespace Useful.Config;

internal static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Failed to read config file '{ConfigPath}'; using defaults.")]
    internal static partial void ConfigReadFailed(ILogger logger, string configPath);

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Config read failure detail for '{ConfigPath}'.")]
    internal static partial void ConfigReadFailedDetail(ILogger logger, string configPath, Exception ex);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "Config file '{ConfigPath}' held values that could not be honoured; those settings are back at their defaults.")]
    internal static partial void ConfigRepaired(ILogger logger, string configPath);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Failed to save config file '{ConfigPath}'.")]
    internal static partial void ConfigWriteFailed(ILogger logger, string configPath, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Kept a copy of the previous config file at '{BackupPath}'.")]
    internal static partial void ConfigBackedUp(ILogger logger, string backupPath);

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, Message = "Could not back up the previous config file to '{BackupPath}'.")]
    internal static partial void ConfigBackupFailed(ILogger logger, string backupPath, Exception ex);
}
