// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib;
using EliteSharpLib.Renditions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Useful.Abstraction.Config;
using Useful.App;

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

    private static ServiceCollection BuildServices(string userDataPath, ILoggerFactory loggerFactory, EngineConfigSettings engine)
    {
        // The rendition is loaded before anything else because it says what
        // size the game draws at, and the window is made at that size. The
        // resolution is the rendition's rather than a setting of its own, so
        // the artwork and the resolution can never disagree.
        InstalledRenditions renditions = EliteServiceCollectionExtensions.LoadRendition(engine.Rendition, loggerFactory);

        ServiceCollection services = new();
        services.AddGameEngine(engine, renditions.Chosen.ScreenWidth, renditions.Chosen.ScreenHeight, Title, loggerFactory);
        services.AddEliteConfig(userDataPath);
        services.AddEliteMain(renditions);

        return services;
    }
}
