// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using EliteSharpLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Useful;
using Useful.Abstraction;
using Useful.Assets;
using Useful.SDL;

[assembly: CLSCompliant(false)]

namespace EliteSharp;

internal static class SDLProgram
{
    private const string Title = "Elite - The Sharp Kind";
    private const string AssetLogCategory = "Assets";

    public static void Main()
    {
        if (!AppStartup.TryResolveUserDataPath(out string userDataPath))
        {
            Environment.Exit(1);
            return;
        }

        using Logger seriLogger = CreateSeriLogger(userDataPath);
        using LoggerFactory loggerFactory = new();
        loggerFactory.AddSerilog(seriLogger);

        using ServiceProvider provider = BuildServices(userDataPath, loggerFactory).BuildServiceProvider();

        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(SDLProgram));

        try
        {
            EliteMain elite = provider.GetRequiredService<EliteMain>();
            LogMessages.StartingTitle(logger, Title);
            elite.Run();
        }
        catch (Exception ex)
        {
            LogMessages.CriticalAppTerminated(logger, ex);
            AppStartup.WriteFailureHint(ex, userDataPath);
            Environment.Exit(-1);
            throw;
        }
    }

    private static Logger CreateSeriLogger(string userDataPath)
    {
        LogEventLevel minimumLevel =
            Enum.TryParse(Environment.GetEnvironmentVariable("ELITE_LOG_LEVEL"), ignoreCase: true, out LogEventLevel envLevel)
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
                formatProvider: CultureInfo.InvariantCulture)
            .WriteTo
            .File(
                Path.Combine(userDataPath, "logs", "elite-.log"),
                formatProvider: CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();
    }

    // Render resolution is a function of the configured tier rather than a
    // separate setting, so the asset set and the resolution can never
    // disagree. Both are the tier's "standard" (non-widescreen) size from
    // docs/decisions.md; 512x512 is what Elite has always rendered at.
    private static (int Width, int Height) ResolutionFor(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => (320, 256),
        _ => (512, 512),
    };

    private static ServiceCollection BuildServices(string userDataPath, ILoggerFactory loggerFactory)
    {
        GraphicsBackend graphicsBackend = EliteServiceCollectionExtensions.ReadGraphicsBackend(userDataPath, loggerFactory);
        SystemTier tier = EliteServiceCollectionExtensions.ReadSystemTier(userDataPath, loggerFactory);
        (int width, int height) = ResolutionFor(tier);

        ServiceCollection services = new();
        services.AddSingleton(loggerFactory);
        services.AddSingleton<IAbstraction>(sp => graphicsBackend == GraphicsBackend.Hardware
            ? new SDLAbstraction(
                width,
                height,
                Title,
                sp.GetRequiredService<IAssetLocator>(),
                sp.GetRequiredService<ILoggerFactory>().CreateLogger(AssetLogCategory))
            : new SoftwareAbstraction(
                width,
                height,
                Title,
                sp.GetRequiredService<IAssetLocator>(),
                sp.GetRequiredService<ILoggerFactory>().CreateLogger(AssetLogCategory)));
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Graphics);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Sound);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Keyboard);
        services.AddSingleton(_ => AssetLocator.Create(tier));
        services.AddEliteConfig(userDataPath);
        services.AddEliteMain();

        return services;
    }
}
