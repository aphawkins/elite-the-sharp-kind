// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Fakes;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using SharpKind.Abstraction;
using SharpKind.Fakes.Input;

namespace EliteSharpLib.Tests.Views;

// The credits screen, with no renderer involved: what it says, and the one
// thing it does.
public class CreditsControllerTests
{
    [Fact]
    public void TheModelCarriesTheVersionAndTheNames()
    {
        CreditsModel model = CreditsController.BuildModel();

        Assert.StartsWith("Version:", model.Version, StringComparison.Ordinal);
        Assert.Contains(model.Credits, credit => credit.Contains("Bell", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(ConsoleKey.Enter)]
    [InlineData(ConsoleKey.Escape)]
    public void BackReturnsToTheOptions(ConsoleKey key)
    {
        CreditsController controller = CreateController(out GameState gameState, out FakeKeyboard keyboard);
        keyboard.KeyDown(key, default);

        controller.HandleInput();

        Assert.Equal(Screen.Options, gameState.CurrentScreen);
    }

    private static CreditsController CreateController(out GameState gameState, out FakeKeyboard keyboard)
    {
        keyboard = new FakeKeyboard();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        views.Add(Screen.Options, new FakeView());
        gameState = new(views, TestMissions.Registry());

        return new CreditsController(gameState, keyboard, new FakeCreditsView());
    }

    private sealed class FakeCreditsView : IView<CreditsModel>
    {
        public void Draw(CreditsModel model)
        {
            // Drawing is not under test here.
        }
    }
}
