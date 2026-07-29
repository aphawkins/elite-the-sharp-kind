// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StuntCarRacerSharpLib;
using Useful.Abstraction.Config;
using Useful.App;

[assembly: CLSCompliant(false)]

namespace StuntCarRacerSharp;

internal static class SDLProgram
{
    private const string Title = "Stunt Car Racer - The Sharp Kind";

    private const int ScreenWidth = 640;
    private const int ScreenHeight = 400;

    public static int Main()
        => GameApp.Run(
            Title,
            logFileName: "scr-.log",
            logLevelEnvironmentVariable: "SCR_LOG_LEVEL",
            StuntCarRacerServiceCollectionExtensions.ReadEngineSettings,
            BuildServices);

    private static ServiceCollection BuildServices(string userDataPath, ILoggerFactory loggerFactory, EngineConfigSettings engine)
    {
        ServiceCollection services = new();
        services.AddGameEngine(engine, ScreenWidth, ScreenHeight, Title, loggerFactory);
        services.AddScrConfig(userDataPath);
        services.AddScrRandom();
        services.AddScrMain();

        return services;
    }
}
