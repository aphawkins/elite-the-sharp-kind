// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Useful.Abstraction;

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
    /// level, so a player can produce a detailed log without a new build.
    /// </param>
    /// <param name="buildServices">
    /// The game's own composition, given the user-data path and the logger
    /// factory. It must register an <see cref="IGameApp"/>.
    /// </param>
    /// <returns>The process exit code.</returns>
    public static int Run(
        string title,
        string logFileName,
        string logLevelEnvironmentVariable,
        Func<string, ILoggerFactory, ServiceCollection> buildServices)
    {
        ArgumentNullException.ThrowIfNull(buildServices);

        if (!AppStartup.TryResolveUserDataPath(out string userDataPath))
        {
            // TryResolveUserDataPath has already reported why to stderr and to
            // the fallback startup log; there is nowhere to write a real log.
            return 1;
        }

        using Logger seriLogger = CreateSeriLogger(userDataPath, logFileName, logLevelEnvironmentVariable);
        using LoggerFactory loggerFactory = new();
        loggerFactory.AddSerilog(seriLogger);

        using ServiceProvider provider = buildServices(userDataPath, loggerFactory).BuildServiceProvider();

        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(GameApp));

        try
        {
            IGameApp game = provider.GetRequiredService<IGameApp>();
            LogMessages.StartingTitle(logger, title);
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

    private static Logger CreateSeriLogger(string userDataPath, string logFileName, string logLevelEnvironmentVariable)
    {
        LogEventLevel minimumLevel =
            Enum.TryParse(
                Environment.GetEnvironmentVariable(logLevelEnvironmentVariable),
                ignoreCase: true,
                out LogEventLevel envLevel)
            ? envLevel
            : LogEventLevel.Information;

        return new LoggerConfiguration()
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
                retainedFileCountLimit: 7)
            .CreateLogger();
    }
}
