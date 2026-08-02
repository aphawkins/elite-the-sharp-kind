// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
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

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "{EnvVar} is {SetState}; starting commander is {Commander}.")]
    internal static partial void DebugCommanderEnvVar(ILogger logger, string envVar, string setState, string commander);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Information,
        Message = "No mission plugin folder at '{Path}'.")]
    internal static partial void NoMissionFolder(ILogger logger, string path);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Warning,
        Message = "Skipped mission plugin '{Path}': it could not be read.")]
    internal static partial void MissionAssemblyUnreadable(ILogger logger, string path, Exception ex);

    [LoggerMessage(
        EventId = 7,
        Level = LogLevel.Information,
        Message = "Loaded {MissionCount} mission(s) from {AssemblyCount} plugin assemblies.")]
    internal static partial void MissionsLoaded(ILogger logger, int missionCount, int assemblyCount);

    [LoggerMessage(
        EventId = 8,
        Level = LogLevel.Critical,
        Message = "'{FirstAssembly}' and '{SecondAssembly}' both provide a mission called '{Name}'.")]
    internal static partial void DuplicateMissionName(ILogger logger, string name, string firstAssembly, string secondAssembly);

    [LoggerMessage(
        EventId = 9,
        Level = LogLevel.Warning,
        Message = "Commander file names mission '{Name}', which nothing provides.")]
    internal static partial void SaveNamesUnknownMission(ILogger logger, string name);

    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Warning,
        Message = "Commander file puts mission '{Name}' at stage '{Stage}', which it does not have.")]
    internal static partial void SaveNamesUnknownStage(ILogger logger, string name, string stage);
}
