// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;
using Useful.Config;

namespace Useful.Abstraction.Config;

/// <summary>
/// Reads the engine half of a game's config file before the DI container that
/// would normally supply it exists.
/// </summary>
/// <remarks>
/// The composition root has to choose a backend, an asset tier and a window
/// size to construct the abstraction, and every one of those comes from the
/// config - but a game's concrete config type is internal to its own library,
/// so the composition root cannot name it. Each game therefore exposes one
/// method that calls this from inside the assembly that can, and gets back the
/// settings that are shared by every game anyway.
/// </remarks>
public static class EngineConfigReader
{
    /// <summary>
    /// Reads and repairs the config file, returning its engine settings.
    /// </summary>
    /// <typeparam name="TConfig">The game's concrete config type.</typeparam>
    /// <param name="userDataPath">The user-data directory the file lives in.</param>
    /// <param name="configFileName">The file's name within that directory.</param>
    /// <param name="repair">The game's repair hook, as passed when registering the file.</param>
    /// <param name="loggerFactory">Where read/repair problems are reported.</param>
    /// <returns>The engine settings held in the file.</returns>
    public static EngineConfigSettings Read<TConfig>(
        string userDataPath,
        string configFileName,
        Func<TConfig, bool> repair,
        ILoggerFactory loggerFactory)
        where TConfig : ConfigSettings, new()
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        ConfigFile<TConfig> configFile = new(
            userDataPath,
            configFileName,
            repair,
            loggerFactory.CreateLogger<ConfigFile<TConfig>>());

        // One read for all of the engine settings: reading each one separately
        // re-read - and could re-repair and rewrite - the whole file per value.
        return configFile.ReadConfig().Engine;
    }
}
