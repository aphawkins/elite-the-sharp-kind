// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// The market's cursor, with no renderer involved. StockType runs 1 (Food) to
// 17 (AlienItems), so the cursor's bounds are clamped to that range rather
// than a plain 0-based one - the original clamped to [0, Count-1], which left
// nothing highlighted at reset (position 0 is StockType.None) and made the
// last row unreachable.
public class MarketControllerTests
{
    [Fact]
    public void ResetHighlightsTheFirstRow()
    {
        MarketController controller = CreateController(out _);

        controller.Reset();

        MarketModel model = controller.BuildModel();
        Assert.Equal("Food", model.Rows[0].Name);
        Assert.True(model.Rows[0].IsHighlighted);
        Assert.DoesNotContain(model.Rows.Skip(1), row => row.IsHighlighted);
    }

    [Fact]
    public void MovingUpAtTheFirstRowStaysThere()
    {
        MarketController controller = CreateController(out FakeKeyboard keyboard);
        controller.Reset();

        keyboard.KeyDown(ConsoleKey.UpArrow, default);
        controller.HandleInput();

        Assert.True(controller.BuildModel().Rows[0].IsHighlighted);
    }

    [Fact]
    public void MovingDownReachesTheLastRow()
    {
        MarketController controller = CreateController(out FakeKeyboard keyboard);
        controller.Reset();

        keyboard.KeyDown(ConsoleKey.DownArrow, default);
        for (int i = 0; i < 20; i++)
        {
            controller.HandleInput();
        }

        MarketModel model = controller.BuildModel();
        Assert.Equal("Alien Items", model.Rows[^1].Name);
        Assert.True(model.Rows[^1].IsHighlighted);
    }

    [Fact]
    public void TitleNamesTheDockedPlanet()
    {
        MarketController controller = CreateController(out _);

        Assert.EndsWith("MARKET PRICES", controller.BuildModel().Title, StringComparison.Ordinal);
    }

    private static MarketController CreateController(out FakeKeyboard keyboard)
    {
        keyboard = new FakeKeyboard();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        GameState gameState = new(views, ClassicMissions.Registry());
        PlayerShip ship = new();
        Trade trade = new(gameState, ship);

        return new MarketController(gameState, keyboard, trade, new PlanetController(gameState), new FakeMarketView());
    }

    private sealed class FakeMarketView : IView<MarketModel>
    {
        public void Draw(MarketModel model)
        {
            // Drawing is not under test here.
        }
    }
}
