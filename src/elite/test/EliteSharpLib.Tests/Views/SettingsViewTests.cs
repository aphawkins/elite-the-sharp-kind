// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
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

public class SettingsViewTests
{
    private const string ConfigFileName = "elitesharp.cfg";

    [Fact]
    public void ChangingASettingSavesItImmediately()
    {
        // Arrange: the view has no save step, so the very first toggle must
        // already be on disk.
        SettingsView view = CreateView(out GameState gameState, out FakeKeyboard keyboard, out ConfigFile<EliteConfigSettings> configFile);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        // Act: item 0 is Ship Style.
        view.HandleInput();

        // Assert
        Assert.True(gameState.Config.ShipWireframe);
        Assert.True(configFile.ReadConfig().ShipWireframe);
    }

    [Fact]
    public void BackReturnsToOptionsWithoutChangingSettings()
    {
        SettingsView view = CreateView(out GameState gameState, out FakeKeyboard keyboard, out _);
        view.Reset();

        // Navigate to the last row - the Back row.
        keyboard.KeyDown(ConsoleKey.DownArrow, default);
        for (int i = 0; i < 4; i++)
        {
            view.HandleInput();
        }

        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        view.HandleInput();

        Assert.Equal(Screen.Options, gameState.CurrentScreen);
        Assert.False(gameState.Config.ShipWireframe);
    }

    private static SettingsView CreateView(
        out GameState gameState,
        out FakeKeyboard keyboard,
        out ConfigFile<EliteConfigSettings> configFile)
    {
        keyboard = new FakeKeyboard();
        ScreenManager<Screen, IView> views = new(keyboard);
        views.Add(Screen.Docking, new FakeView());
        views.Add(Screen.GameOver, new FakeView());
        views.Add(Screen.Hyperspace, new FakeView());
        views.Add(Screen.Options, new FakeView());
        gameState = new(views);

        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource());
        PlayerShip ship = new();
        Trade trade = new(gameState, ship);
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        Combat combat = new(gameState, audio, ship, trade, pilot, universe, draw, shipFactory, rng);
        Space space = new(
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

        configFile = new(
            Path.Combine(Path.GetTempPath(), "EliteSettingsViewTests_" + Guid.NewGuid().ToString("N")),
            ConfigFileName);

        return new SettingsView(gameState, draw, keyboard, space, configFile);
    }
}
