// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Config;
using Useful.Fakes;
using Useful.Fakes.Audio;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// Shared setup for the two settings screens: both need a real Space (they
// rebuild the planet and sun in place) over a fake draw surface.
internal static class SettingsControllerFixture
{
    internal static Space CreateSpace(
        out GameState gameState,
        out FakeKeyboard keyboard,
        out FakeEliteDraw draw,
        out AudioController audio)
    {
        keyboard = new FakeKeyboard();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        views.Add(Screen.Docking, new FakeView());
        views.Add(Screen.GameOver, new FakeView());
        views.Add(Screen.Hyperspace, new FakeView());
        views.Add(Screen.Options, new FakeView());
        gameState = new(views, ClassicMissions.Registry());

        draw = new FakeEliteDraw();
        RNG rng = new(new FakeRandomSource());
        PlayerShip ship = new();
        Trade trade = new(gameState, ship);
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);
        audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        Combat combat = new(gameState, audio, ship, trade, pilot, universe, draw, shipFactory, rng);

        return new Space(
            gameState,
            audio,
            pilot,
            combat,
            trade,
            ship,
            new PlanetController(gameState),
            new Stars(gameState, draw, ship, rng),
            universe,
            draw,
            rng);
    }

    internal static ConfigFile<EliteConfig> CreateConfigFile(string configFileName)
        => new(
            Path.Combine(Path.GetTempPath(), "EliteSettingsViewTests_" + Guid.NewGuid().ToString("N")),
            configFileName);
}
