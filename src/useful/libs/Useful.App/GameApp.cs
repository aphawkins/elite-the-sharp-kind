// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Useful.Abstraction;
using Useful.Abstraction.Config;

namespace Useful.App;

/// <summary>
/// The shared half of a game executable's <c>Program.Main</c>: resolve the
/// user-data directory, stand up logging, build the container, run the game,
/// and report a failure in a way a player can act on.
/// </summary>
/// <remarks>
/// This is app-layer policy, not library code - it is why <c>Useful.App</c>
/// exists as an assembly no game library references. Each executable supplies
/// only what actually differs between the games: its title, its log file, its
/// log-level environment variable, and its own service registrations.
/// </remarks>
public static class GameApp
{
    /// <summary>
    /// Runs a game to completion and returns the process exit code: zero for a
    /// normal exit, non-zero if the user-data directory could not be resolved
    /// or the game terminated unexpectedly. Returning the code rather than
    /// calling <see cref="Environment.Exit(int)"/> lets <c>Main</c> unwind, so
    /// the container disposes its singletons on the way out.
    /// </summary>
    /// <param name="title">The window/log title, as logged at startup.</param>
    /// <param name="logFileName">
    /// The rolling log file's name within the user-data <c>logs</c> directory;
    /// Serilog inserts the date before the extension, so this is a template
    /// like <c>elite-.log</c> rather than a literal filename.
    /// </param>
    /// <param name="logLevelEnvironmentVariable">
    /// Names the environment variable that raises or lowers the minimum log
    /// level, overriding both the default and whatever the config file says -
    /// the escape hatch for when the config file itself is what needs
    /// debugging.
    /// </param>
    /// <param name="readEngineSettings">
    /// Reads the game's config file and returns its engine half, given the
    /// user-data path and a logger for the read itself. Called before the
    /// real logger exists (it has no file-retention setting to honour yet),
    /// so it gets a console-only bootstrap logger.
    /// </param>
    /// <param name="buildServices">
    /// The game's own composition, given the user-data path, the real
    /// logger factory and the already-read engine settings. It must register
    /// an <see cref="IGameApp"/>.
    /// </param>
    /// <returns>The process exit code.</returns>
    public static int Run(
        string title,
        string logFileName,
        string logLevelEnvironmentVariable,
        Func<string, ILoggerFactory, EngineConfigSettings> readEngineSettings,
        Func<string, ILoggerFactory, EngineConfigSettings, ServiceCollection> buildServices)
    {
        ArgumentNullException.ThrowIfNull(readEngineSettings);
        ArgumentNullException.ThrowIfNull(buildServices);

        if (!AppStartup.TryResolveUserDataPath(out string userDataPath))
        {
            // TryResolveUserDataPath has already reported why to stderr and to
            // the fallback startup log; there is nowhere to write a real log.
            return 1;
        }

        LogEventLevel? environmentLevel = ReadEnvironmentLevel(logLevelEnvironmentVariable);
        EngineConfigSettings engine = ReadEngineSettings(userDataPath, environmentLevel, readEngineSettings);

        LogEventLevel minimumLevel = environmentLevel ?? LevelConvert.ToSerilogLevel(engine.Logging.MinimumLevel);

        using Logger seriLogger = CreateSeriLogger(userDataPath, logFileName, minimumLevel, engine.Logging.RetainedFileCount);
        using LoggerFactory loggerFactory = new();
        loggerFactory.AddSerilog(seriLogger);

        using ServiceProvider provider = buildServices(userDataPath, loggerFactory, engine).BuildServiceProvider();

        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(GameApp));

        LogMessages.StartingTitle(logger, title);
        LogStartupDiagnostics(logger, engine);

        try
        {
            IGameApp game = provider.GetRequiredService<IGameApp>();
            game.Run();
        }
        catch (Exception ex)
        {
            // The exception is logged in full above, so the player gets a hint
            // and a non-zero exit rather than a raw stack dump on the console -
            // which is what the previous Environment.Exit(-1) achieved by
            // terminating before the rethrow could surface.
            LogMessages.CriticalAppTerminated(logger, ex);
            AppStartup.WriteFailureHint(ex, userDataPath);
            return -1;
        }

        return 0;
    }

    // A bug report rarely comes with the reporter's machine spec or config
    // file attached, so the log itself carries what a fix usually needs
    // first: which build, which OS/runtime, and which engine settings were
    // in effect. Shared by every game via this one call. Logged as JSON
    // (rather than a prose sentence) so the two facts can be machine-parsed
    // back out of the log file.
    private static void LogStartupDiagnostics(Microsoft.Extensions.Logging.ILogger logger, EngineConfigSettings engine)
    {
        string version = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?? "unknown";

        var systemInfo = new
        {
            version,
            os = RuntimeInformation.OSDescription,
            runtime = RuntimeInformation.FrameworkDescription,
            architecture = RuntimeInformation.ProcessArchitecture.ToString(),
            processorCount = Environment.ProcessorCount,
        };
        string systemInfoJson = JsonSerializer.Serialize(systemInfo);
        LogMessages.SystemInfo(logger, systemInfoJson);

        var engineSettings = new
        {
            backend = engine.Backend.ToString(),
            tier = engine.Tier.ToString(),
            windowScale = engine.WindowScale,
            fps = engine.Graphics.Fps,
            graphicStyle = engine.Graphics.GraphicStyle.ToString(),
            depthSort = engine.Graphics.DepthSort.ToString(),
            soundEffects = engine.Sound.Effects,
            soundMusic = engine.Sound.Music,
        };
        string engineSettingsJson = JsonSerializer.Serialize(engineSettings);
        LogMessages.EngineSettings(logger, engineSettingsJson);
    }

    // Null means the environment variable was unset or unparseable; the
    // caller falls back to the config value (and ultimately its default)
    // rather than a level of its own.
    private static LogEventLevel? ReadEnvironmentLevel(string logLevelEnvironmentVariable)
        => Enum.TryParse(
            Environment.GetEnvironmentVariable(logLevelEnvironmentVariable),
            ignoreCase: true,
            out LogEventLevel envLevel)
            ? envLevel
            : null;

    // The engine's Logging settings live in the config file the game itself
    // reads, but reading it needs a logger - one that cannot yet know the
    // config's own retained-file-count, since that is exactly what it is
    // about to read. A console-only bootstrap logger breaks the cycle: it
    // never touches the log file, so it needs no retention setting, and its
    // level is already fully known (the environment variable, or the
    // default) without the config.
    private static EngineConfigSettings ReadEngineSettings(
        string userDataPath,
        LogEventLevel? environmentLevel,
        Func<string, ILoggerFactory, EngineConfigSettings> readEngineSettings)
    {
        using Logger bootstrapLogger = CreateBootstrapLogger(environmentLevel ?? LogEventLevel.Information);
        using LoggerFactory bootstrapLoggerFactory = new();
        bootstrapLoggerFactory.AddSerilog(bootstrapLogger);

        return readEngineSettings(userDataPath, bootstrapLoggerFactory);
    }

    private static Logger CreateBootstrapLogger(LogEventLevel minimumLevel)
        => new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .MinimumLevel
            .Is(minimumLevel)
            .WriteTo
            .Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}",
                formatProvider: System.Globalization.CultureInfo.InvariantCulture)
            .CreateLogger();

    private static Logger CreateSeriLogger(string userDataPath, string logFileName, LogEventLevel minimumLevel, int retainedFileCount)
        => new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .MinimumLevel
            .Is(minimumLevel)
            .WriteTo
            .Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}",
                formatProvider: System.Globalization.CultureInfo.InvariantCulture)
            .WriteTo
            .File(
                Path.Combine(userDataPath, "logs", logFileName),
                formatProvider: System.Globalization.CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: retainedFileCount)
            .CreateLogger();
}
