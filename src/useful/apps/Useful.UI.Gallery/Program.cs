// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Useful.Abstraction;
using Useful.Abstraction.Config;
using Useful.App;

[assembly: CLSCompliant(false)]

namespace Useful.UI.Gallery;

/// <summary>
/// Opens a window showing every control and lets the keyboard drive the ones
/// that take input.
/// <para>
/// Built on <see cref="GameApp"/> like the two games, rather than standing a
/// window up for itself: the gallery then gets the same backend choice, the
/// same window scaling, the same logging and the same failure reporting they
/// do - and, through <c>GameHost</c>, the <c>GAME_KEY_SCRIPT</c> and
/// <c>GAME_FRAME_DUMP_DIR</c> facilities, which are how a control can be
/// driven and photographed without injecting keys at the OS.
/// </para>
/// </summary>
internal static class Program
{
    private const string Title = "Useful UI - Gallery";
    private const string ConfigFileName = "gallery.sharp";

    // The 8-bit tier's canvas width, so what shows here is what that tier
    // draws. The height is in rows rather than pixels: the gallery lays itself
    // out a row at a time, and a row is as tall as the font the backend
    // actually uses - 8 pixels from the bitmap sheet the software renderer
    // draws from, around twice that from the 12pt true-type face the hardware
    // one does. A canvas fixed in pixels would crop half the gallery on one of
    // them.
    private const int Width = 320;
    private const int BitmapRowHeight = 8;

    // Rounded up from the 12pt face's real line height rather than down: the
    // gallery holds itself to whatever the canvas can show, so slack at the
    // foot costs a band of empty pixels while a shortfall would cost a row.
    private const int TrueTypeRowHeight = 18;

    // Keeps the window on a laptop screen: a canvas of taller rows magnified
    // as far as a canvas of short ones would be goes off the bottom of the
    // display, which no window scale in the config file can be blamed for.
    private const int MaxWindowHeight = 900;

    public static int Main()
        => GameApp.Run(
            Title,
            logFileName: "gallery-.log",
            logLevelEnvironmentVariable: "GALLERY_LOG_LEVEL",
            ReadEngineSettings,
            BuildServices);

    private static EngineConfigSettings ReadEngineSettings(string userDataPath, ILoggerFactory loggerFactory)
        => EngineConfigReader.Read<GalleryConfig>(userDataPath, ConfigFileName, RepairConfig, loggerFactory);

    // The engine settings repair themselves; the gallery adds nothing that
    // could need it.
    private static bool RepairConfig(GalleryConfig config) => config.Repair();

    private static ServiceCollection BuildServices(
        string userDataPath,
        ILoggerFactory loggerFactory,
        EngineConfigSettings engine)
    {
        int height = Gallery.LayoutRows * (engine.Backend == Backend.Hardware ? TrueTypeRowHeight : BitmapRowHeight);

        while (engine.WindowScale > 1 && height * engine.WindowScale > MaxWindowHeight)
        {
            engine.WindowScale--;
        }

        ServiceCollection services = new();
        services.AddGameEngine(engine, Width, height, Title, loggerFactory);
        services.AddSingleton(engine);
        services.AddSingleton<IGameApp>(sp => new GalleryMain(
            sp.GetRequiredService<IAbstraction>(),
            sp.GetRequiredService<EngineConfigSettings>()));

        return services;
    }
}
