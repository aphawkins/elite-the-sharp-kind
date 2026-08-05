// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Config;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Renditions;
using EliteSharpLib.Views;
using Useful.Audio;
using Useful.Config;
using Useful.Fakes.Controls;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests.Views;

public class EngineSettingsControllerTests
{
    private const string ConfigFileName = "elite.sharp";

    [Fact]
    public void ChangingASettingSavesItImmediately()
    {
        // Arrange: as the game settings screen, there's no save step.
        EngineSettingsController controller = CreateController(
            out GameState gameState, out FakeKeyboard keyboard, out _, out ConfigFile<EliteConfig> configFile);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        // Act: item 0 is Graphic Style.
        controller.HandleInput();

        // Assert
        Assert.Equal(GraphicStyle.Wireframe, gameState.Config.Engine.Graphics.GraphicStyle);
        Assert.Equal(GraphicStyle.Wireframe, configFile.ReadConfig().Engine.Graphics.GraphicStyle);
    }

    // Row 2 is Music: the config and the running AudioController have to move
    // together, or the setting only takes effect after a restart.
    [Fact]
    public void TurningMusicOffAppliesToTheRunningAudioController()
    {
        EngineSettingsController controller = CreateController(
            out GameState gameState, out FakeKeyboard keyboard, out AudioController audio, out _);
        controller.Reset();

        // Down twice to row 2, then toggle.
        for (int i = 0; i < 2; i++)
        {
            keyboard.KeyDown(ConsoleKey.DownArrow, default);
            controller.HandleInput();
        }

        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();

        Assert.False(gameState.Config.Engine.Sound.Music);
        Assert.False(audio.MusicOn);
    }

    // The game's own settings belong to the other screen.
    [Fact]
    public void ChangingAnEngineSettingLeavesTheGameSettingsAlone()
    {
        EngineSettingsController controller = CreateController(out GameState gameState, out FakeKeyboard keyboard, out _, out _);
        keyboard.KeyDown(ConsoleKey.Enter, default);

        controller.HandleInput();

        Assert.Equal(PlanetType.Fractal, gameState.Config.Game.PlanetStyle);
    }

    [Fact]
    public void BackReturnsToOptionsWithoutChangingSettings()
    {
        EngineSettingsController controller = CreateController(out GameState gameState, out FakeKeyboard keyboard, out _, out _);
        controller.Reset();

        // Navigate to the last row - the Back row.
        for (int i = 0; i < 6; i++)
        {
            keyboard.KeyDown(ConsoleKey.DownArrow, default);
            controller.HandleInput();
        }

        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();

        Assert.Equal(Screen.Options, gameState.CurrentScreen);
        Assert.Equal(GraphicStyle.Solid, gameState.Config.Engine.Graphics.GraphicStyle);
    }

    private static EngineSettingsController CreateController(
        out GameState gameState,
        out FakeKeyboard keyboard,
        out AudioController audio,
        out ConfigFile<EliteConfig> configFile)
    {
        Space space = SettingsControllerFixture.CreateSpace(out gameState, out keyboard, out FakeEliteDraw draw, out audio);
        configFile = SettingsControllerFixture.CreateConfigFile(ConfigFileName);

        return new EngineSettingsController(
            gameState,
            keyboard,
            space,
            audio,
            configFile,
            new InstalledRenditions(new SixteenBitRendition(), string.Empty, ["8-bit", "16-bit"]),
            SettingsControllerFixture.CreateBaseView(draw),
            draw,
            SettingsControllerFixture.CreateStyle(draw));
    }
}
