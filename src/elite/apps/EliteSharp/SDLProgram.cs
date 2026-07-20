// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using EliteSharp.SDL;
using EliteSharpLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Useful.Abstraction;
using Useful.Assets;
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
        LogEventLevel minimumLevel =
            Enum.TryParse(Environment.GetEnvironmentVariable("ELITE_LOG_LEVEL"), ignoreCase: true, out LogEventLevel envLevel)
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
                Path.Combine("logs", "elite-.log"),
                formatProvider: CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        using LoggerFactory loggerFactory = new();
        loggerFactory.AddSerilog(seriLogger);

        string userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EliteSharp");

        ServiceCollection services = new();
        services.AddSingleton<ILoggerFactory>(loggerFactory);
        services.AddSingleton<IAbstraction>(_ => new SoftwareAbstraction(ScreenWidth, ScreenHeight, Title));
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Graphics);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Sound);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Keyboard);
        services.AddSingleton(_ => AssetLocator.Create());
        services.AddEliteConfig(userDataPath);
        services.AddEliteMain();

        using ServiceProvider provider = services.BuildServiceProvider();

        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(SDLProgram));

        EliteMain elite = provider.GetRequiredService<EliteMain>();

        try
        {
            LogMessages.StartingTitle(logger, Title);
            elite.Run();
        }
        catch (Exception ex)
        {
            LogMessages.CriticalAppTerminated(logger, ex);
            throw;
        }
    }
}
