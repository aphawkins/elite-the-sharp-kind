// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Input;

namespace EliteSharpLib.Tests.Views;

// The options menu's row-enabled state, with no renderer involved - a
// docked-only row is greyed out (not hidden) while undocked, so the model
// carries that as a per-row flag rather than a shorter list.
public class OptionsControllerTests
{
    [Fact]
    public void DockedOnlyRowsAreEnabledWhileDocked()
    {
        OptionsController controller = CreateController(out GameState gameState);
        gameState.IsDocked = true;

        Assert.All(controller.BuildModel().Options, row => Assert.True(row.IsEnabled));
    }

    [Fact]
    public void DockedOnlyRowsAreDisabledWhileUndocked()
    {
        OptionsController controller = CreateController(out GameState gameState);
        gameState.IsDocked = false;

        OptionsModel model = controller.BuildModel();

        Assert.False(model.Options[0].IsEnabled); // Save Commander
        Assert.False(model.Options[1].IsEnabled); // Load Commander
        Assert.True(model.Options[2].IsEnabled); // Game Settings
    }

    [Fact]
    public void ResetPutsTheCursorOnTheFirstRow()
    {
        OptionsController controller = CreateController(out _);

        controller.Reset();

        Assert.Equal(0, controller.BuildModel().HighlightedIndex);
    }

    private static OptionsController CreateController(out GameState gameState)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        gameState = new(views, TestMissions.Registry());

        return new OptionsController(gameState, new FakeKeyboard(), new FakeOptionsView());
    }

    private sealed class FakeOptionsView : IView<OptionsModel>
    {
        public void Draw(OptionsModel model)
        {
            // Drawing is not under test here.
        }
    }
}
