// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Useful.Abstraction.Config;
using Useful.App;
using Useful.Assets;

[assembly: CLSCompliant(false)]

namespace EliteSharp;

internal static class SDLProgram
{
    private const string Title = "Elite - The Sharp Kind";

    public static int Main()
        => GameApp.Run(
            Title,
            logFileName: "elite-.log",
            logLevelEnvironmentVariable: "ELITE_LOG_LEVEL",
            EliteServiceCollectionExtensions.ReadEngineSettings,
            BuildServices);

    // Render resolution is a function of the configured tier rather than a
    // separate setting, so the asset set and the resolution can never
    // disagree. Both are the tier's "standard" (non-widescreen) size from
    // docs/decisions.md. The 16-bit tier widened from 512x512 to 640x512 on
    // 2026-07-30, alongside a 640-wide scanner; the height is unchanged, so
    // the vertical field of view is too (Focus follows ScreenHeight).
    private static (int Width, int Height) ResolutionFor(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => (320, 256),
        _ => (640, 512),
    };

    private static ServiceCollection BuildServices(string userDataPath, ILoggerFactory loggerFactory, EngineConfigSettings engine)
    {
        (int width, int height) = ResolutionFor(engine.Tier);

        ServiceCollection services = new();
        services.AddGameEngine(engine, width, height, Title, loggerFactory);
        services.AddEliteConfig(userDataPath);
        services.AddEliteMain();

        return services;
    }
}
