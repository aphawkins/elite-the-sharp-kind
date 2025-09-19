// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

[assembly: CLSCompliant(false)]

namespace EliteSharp.SDL;

internal static class SDLProgram
{
    private const string Title = "Elite - The Sharp Kind";

#if QHD
    private const int ScreenWidth = 960;
    private const int ScreenHeight = 540;
#else
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
#endif

#if SOFTWARERENDERER
    private static readonly SDLGameFactory s_gameFactory = new(ScreenWidth, ScreenHeight, Title, "SOFTWARE");
#else
    private static readonly SDLGameFactory s_gameFactory = new(ScreenWidth, ScreenHeight, Title, "SDL");
#endif

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

        try
        {
            LogMessages.StartingTitle(logger, Title);
            s_gameFactory.Game.Run();
        }
        catch (Exception ex)
        {
            LogMessages.CriticalAppTerminated(logger, ex);
            Environment.Exit(-1);
            throw;
        }
    }
}
