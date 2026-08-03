// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Fakes;
using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// The save-commander screen's behaviour, with no renderer involved: typing,
// then a status message that depends on whether the save worked.
public class SaveCommanderControllerTests
{
    [Fact]
    public void ResetStartsWithTheCurrentCommandersNameAndNoMessage()
    {
        SaveCommanderController controller = CreateController(out GameState gameState, out _, out _);
        gameState.Cmdr.Name = "MAX";

        controller.Reset();

        SaveCommanderModel model = controller.BuildModel();
        Assert.Equal("MAX", model.Name);
        Assert.Equal(string.Empty, model.StatusMessage);
    }

    [Fact]
    public void SavingShowsASuccessMessage()
    {
        SaveCommanderController controller = CreateController(out _, out FakeKeyboard keyboard, out _);
        controller.Reset();

        Type(controller, keyboard, "MAX");
        PressEnter(controller, keyboard);

        Assert.Equal("Commander Saved.", controller.BuildModel().StatusMessage);
    }

    [Fact]
    public void SpaceReturnsToOptions()
    {
        SaveCommanderController controller = CreateController(out GameState gameState, out FakeKeyboard keyboard, out _);
        controller.Reset();

        keyboard.KeyDown(ConsoleKey.Spacebar, default);
        controller.HandleInput();

        Assert.Equal(Screen.Options, gameState.CurrentScreen);
    }

    private static void Type(SaveCommanderController controller, FakeKeyboard keyboard, string text)
    {
        foreach (char c in text)
        {
            keyboard.KeyDown((ConsoleKey)c, default);
            controller.HandleInput();
            keyboard.KeyUp((ConsoleKey)c, default);
        }
    }

    private static void PressEnter(SaveCommanderController controller, FakeKeyboard keyboard)
    {
        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();
        keyboard.KeyUp(ConsoleKey.Enter, default);
    }

    private static SaveCommanderController CreateController(
        out GameState gameState, out FakeKeyboard keyboard, out SaveFile save)
    {
        keyboard = new FakeKeyboard();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        views.Add(Screen.Options, new FakeView());
        gameState = new(views, TestMissions.Registry());
        PlayerShip ship = new();
        Trade trade = new(gameState, ship);
        string directory = Path.Combine(Path.GetTempPath(), "SaveCommanderControllerTests_" + Guid.NewGuid().ToString("N"));
        save = new SaveFile(gameState, ship, trade, new PlanetController(gameState), TestMissions.Registry(), directory);

        return new SaveCommanderController(gameState, keyboard, save, new FakeSaveCommanderView());
    }

    private sealed class FakeSaveCommanderView : IView<SaveCommanderModel>
    {
        public void Draw(SaveCommanderModel model)
        {
            // Drawing is not under test here.
        }
    }
}
