// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using StuntCarRacerSharpLib;
using Useful;
using Useful.Abstraction;
using Useful.Audio;
using Useful.SDL;

[assembly: CLSCompliant(false)]

namespace StuntCarRacerSharp;

internal static class SDLProgram
{
    private const string Title = "Stunt Car Racer - The Sharp Kind";

    private const int ScreenWidth = 640;
    private const int ScreenHeight = 400;

    public static void Main()
    {
        string userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TheSharpKind");

        LogEventLevel minimumLevel =
            Enum.TryParse(Environment.GetEnvironmentVariable("SCR_LOG_LEVEL"), ignoreCase: true, out LogEventLevel envLevel)
            ? envLevel
            : LogEventLevel.Information;

        Logger seriLogger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .MinimumLevel
            .Is(minimumLevel)
            .WriteTo
            .Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo
            .File(
                Path.Combine(userDataPath, "logs", "scr-.log"),
                formatProvider: CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        using LoggerFactory loggerFactory = new();
        loggerFactory.AddSerilog(seriLogger);

        ServiceCollection services = new();
        services.AddSingleton<ILoggerFactory>(loggerFactory);
        services.AddSingleton<IAbstraction>(_ => new SoftwareAbstraction(ScreenWidth, ScreenHeight, Title));
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Graphics);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Sound);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Keyboard);
        services.AddScrConfig(userDataPath);
        services.AddScrRandom();
        services.AddSingleton(sp => new StuntCarRacerMain(
            sp.GetRequiredService<IAbstraction>(),
            sp.GetRequiredService<AudioOptions>(),
            sp.GetRequiredService<IRandomSource>()));
        services.AddSingleton<IGame>(sp => sp.GetRequiredService<StuntCarRacerMain>());

        using ServiceProvider provider = services.BuildServiceProvider();

        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(SDLProgram));

        try
        {
            StuntCarRacerMain game = provider.GetRequiredService<StuntCarRacerMain>();
            LogMessages.StartingTitle(logger, Title);
            game.Run();
        }
        catch (Exception ex)
        {
            LogMessages.CriticalAppTerminated(logger, ex);
            Environment.Exit(-1);
            throw;
        }
    }
}
