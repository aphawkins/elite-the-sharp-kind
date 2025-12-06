// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using EliteSharp.SDL;
using EliteSharpLib;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Useful.SDL;

[assembly: CLSCompliant(false)]

namespace EliteSharp;

internal static class SDLProgram
{
    private const string Title = "Elite - The Sharp Kind";

    // Get these from config
    ////#if QHD
    ////    private const int ScreenWidth = 960;
    ////    private const int ScreenHeight = 540;
    ////#else
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
    ////#endif

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

        // TODO: Use DI to provide the abstraction
        using SoftwareAbstraction abstraction = new(ScreenWidth, ScreenHeight, Title);
        ////IAssetLocator assetLocator = AssetLocator.Create();
        ////using SDLAbstraction abstraction = new(ScreenWidth, ScreenHeight, Title, assetLocator);

        EliteMain elite = new(abstraction);

        try
        {
            LogMessages.StartingTitle(logger, Title);
            elite.Run();
        }
        catch (Exception ex)
        {
            LogMessages.CriticalAppTerminated(logger, ex);
            Environment.Exit(-1);
            throw;
        }
    }
}
