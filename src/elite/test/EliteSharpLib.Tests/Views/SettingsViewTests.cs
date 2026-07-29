// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Views;
using Useful.Config;
using Useful.Fakes.Controls;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests.Views;

public class SettingsViewTests
{
    private const string ConfigFileName = "elite.sharp";

    [Fact]
    public void ChangingASettingSavesItImmediately()
    {
        // Arrange: the view has no save step, so the very first toggle must
        // already be on disk.
        SettingsView view = CreateView(out GameState gameState, out FakeKeyboard keyboard, out ConfigFile<EliteConfig> configFile);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        // Act: item 0 is Planet Style, which starts at Fractal and wraps.
        view.HandleInput();

        // Assert
        Assert.Equal(PlanetType.Solid, gameState.Config.Game.PlanetStyle);
        Assert.Equal(PlanetType.Solid, configFile.ReadConfig().Game.PlanetStyle);
    }

    // The engine's settings belong to the other screen.
    [Fact]
    public void ChangingAGameSettingLeavesTheEngineSettingsAlone()
    {
        SettingsView view = CreateView(out GameState gameState, out FakeKeyboard keyboard, out _);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        view.HandleInput();

        Assert.Equal(GraphicStyle.Solid, gameState.Config.Engine.Graphics.GraphicStyle);
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
        Assert.Equal(PlanetType.Fractal, gameState.Config.Game.PlanetStyle);
    }

    private static SettingsView CreateView(
        out GameState gameState,
        out FakeKeyboard keyboard,
        out ConfigFile<EliteConfig> configFile)
    {
        Space space = SettingsViewFixture.CreateSpace(out gameState, out keyboard, out FakeEliteDraw draw, out _);
        configFile = SettingsViewFixture.CreateConfigFile(ConfigFileName);

        return new SettingsView(gameState, draw, keyboard, space, configFile);
    }
}
