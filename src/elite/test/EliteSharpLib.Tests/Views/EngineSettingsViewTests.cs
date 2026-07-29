// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Views;
using Useful.Audio;
using Useful.Config;
using Useful.Fakes.Controls;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests.Views;

public class EngineSettingsViewTests
{
    private const string ConfigFileName = "elite.sharp";

    [Fact]
    public void ChangingASettingSavesItImmediately()
    {
        // Arrange: as the game settings screen, there's no save step.
        EngineSettingsView view = CreateView(
            out GameState gameState, out FakeKeyboard keyboard, out _, out ConfigFile<EliteConfig> configFile);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        // Act: item 0 is Graphic Style.
        view.HandleInput();

        // Assert
        Assert.Equal(GraphicStyle.Wireframe, gameState.Config.Engine.Graphics.GraphicStyle);
        Assert.Equal(GraphicStyle.Wireframe, configFile.ReadConfig().Engine.Graphics.GraphicStyle);
    }

    // Row 2 is Music: the config and the running AudioController have to move
    // together, or the setting only takes effect after a restart.
    [Fact]
    public void TurningMusicOffAppliesToTheRunningAudioController()
    {
        EngineSettingsView view = CreateView(
            out GameState gameState, out FakeKeyboard keyboard, out AudioController audio, out _);
        view.Reset();

        // Down once to row 2, then toggle.
        keyboard.KeyDown(ConsoleKey.DownArrow, default);
        view.HandleInput();
        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        view.HandleInput();

        Assert.False(gameState.Config.Engine.Sound.Music);
        Assert.False(audio.MusicOn);
    }

    // The game's own settings belong to the other screen.
    [Fact]
    public void ChangingAnEngineSettingLeavesTheGameSettingsAlone()
    {
        EngineSettingsView view = CreateView(out GameState gameState, out FakeKeyboard keyboard, out _, out _);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        view.HandleInput();

        Assert.Equal(PlanetType.Fractal, gameState.Config.Game.PlanetStyle);
    }

    [Fact]
    public void BackReturnsToOptionsWithoutChangingSettings()
    {
        EngineSettingsView view = CreateView(out GameState gameState, out FakeKeyboard keyboard, out _, out _);
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
        Assert.Equal(GraphicStyle.Solid, gameState.Config.Engine.Graphics.GraphicStyle);
    }

    private static EngineSettingsView CreateView(
        out GameState gameState,
        out FakeKeyboard keyboard,
        out AudioController audio,
        out ConfigFile<EliteConfig> configFile)
    {
        Space space = SettingsViewFixture.CreateSpace(out gameState, out keyboard, out FakeEliteDraw draw, out audio);
        configFile = SettingsViewFixture.CreateConfigFile(ConfigFileName);

        return new EngineSettingsView(gameState, draw, keyboard, space, audio, configFile);
    }
}
