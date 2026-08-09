// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Config;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Renditions;
using EliteSharpLib.Views;
using SharpKind.Audio;
using SharpKind.Config;
using SharpKind.Fakes.Input;
using SharpKind.Graphics.Rendering;

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

        // Act: item 0 is Fill Mode.
        controller.HandleInput();

        // Assert
        Assert.Equal(FillMode.Wireframe, gameState.Config.Engine.Graphics.FillMode);
        Assert.Equal(FillMode.Wireframe, configFile.ReadConfig().Engine.Graphics.FillMode);
    }

    // Row 2 is Shading, which defaults Unlit - the flat original is what ships
    // - so stepping it once selects Lambert and saves that.
    [Fact]
    public void SelectingLambertShadingSavesIt()
    {
        EngineSettingsController controller = CreateController(
            out GameState gameState, out FakeKeyboard keyboard, out _, out ConfigFile<EliteConfig> configFile);
        controller.Reset();

        for (int i = 0; i < 2; i++)
        {
            keyboard.KeyDown(ConsoleKey.DownArrow, default);
            controller.HandleInput();
        }

        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();

        Assert.Equal(ShadingModelKind.Lambert, gameState.Config.Engine.Graphics.Shading);
        Assert.Equal(ShadingModelKind.Lambert, configFile.ReadConfig().Engine.Graphics.Shading);
    }

    // Row 3 is Quantisation, added alongside Shading as the pipeline's output
    // stage - the two are separate choices, so both get their own row.
    [Fact]
    public void SelectingOrderedQuantisationSavesIt()
    {
        EngineSettingsController controller = CreateController(
            out GameState gameState, out FakeKeyboard keyboard, out _, out ConfigFile<EliteConfig> configFile);
        controller.Reset();

        for (int i = 0; i < 3; i++)
        {
            keyboard.KeyDown(ConsoleKey.DownArrow, default);
            controller.HandleInput();
        }

        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();

        Assert.Equal(Quantisation.Ordered, gameState.Config.Engine.Graphics.Quantisation);
        Assert.Equal(Quantisation.Ordered, configFile.ReadConfig().Engine.Graphics.Quantisation);
    }

    // Row 4 is Music: the config and the running AudioController have to move
    // together, or the setting only takes effect after a restart.
    [Fact]
    public void TurningMusicOffAppliesToTheRunningAudioController()
    {
        EngineSettingsController controller = CreateController(
            out GameState gameState, out FakeKeyboard keyboard, out AudioController audio, out _);
        controller.Reset();

        // Down four times to the Music row, then toggle.
        for (int i = 0; i < 4; i++)
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
        for (int i = 0; i < 8; i++)
        {
            keyboard.KeyDown(ConsoleKey.DownArrow, default);
            controller.HandleInput();
        }

        keyboard.KeyUp(ConsoleKey.DownArrow, default);
        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();

        Assert.Equal(Screen.Options, gameState.CurrentScreen);
        Assert.Equal(FillMode.Solid, gameState.Config.Engine.Graphics.FillMode);
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
