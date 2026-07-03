// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using StuntCarRacerLib;
using Useful.SDL;

[assembly: CLSCompliant(false)]

namespace StuntCarRacer;

internal static class SDLProgram
{
    private const string Title = "Stunt Car Racer - The Sharp Kind";

    private const int ScreenWidth = 640;
    private const int ScreenHeight = 400;

    public static void Main()
    {
        Logger seriLogger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .MinimumLevel
            .Verbose()
            .WriteTo
            .Debug(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();

        using LoggerFactory loggerFactory = new();
        loggerFactory.AddSerilog(seriLogger);

        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(SDLProgram));

        using SoftwareAbstraction abstraction = new(ScreenWidth, ScreenHeight, Title);

        StuntCarRacerMain game = new(abstraction);

        try
        {
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
