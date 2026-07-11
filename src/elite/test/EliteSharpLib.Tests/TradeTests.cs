// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests;

public class TradeTests
{
    [Fact]
    public void IsCarryingContrabandCountsSlavesAndNarcoticsOnce()
    {
        // Arrange
        Trade trade = CreateTrade();
        trade.StockMarket[StockType.Slaves].CurrentCargo = 2;
        trade.StockMarket[StockType.Narcotics].CurrentCargo = 3;
        trade.StockMarket[StockType.Firearms].CurrentCargo = 1;

        // Act
        int contraband = trade.IsCarryingContraband();

        // Assert
        Assert.Equal(((2 + 3) * 2) + 1, contraband);
    }

    [Fact]
    public void IsCarryingContrabandIsZeroWithNoContrabandCargo()
    {
        // Arrange
        Trade trade = CreateTrade();
        trade.StockMarket[StockType.Food].CurrentCargo = 10;

        // Act
        int contraband = trade.IsCarryingContraband();

        // Assert
        Assert.Equal(0, contraband);
    }

    private static Trade CreateTrade()
    {
        ScreenManager<Screen, IView> views = new(new FakeKeyboard());
        GameState gameState = new(views);
        PlayerShip ship = new();
        return new Trade(gameState, ship);
    }
}
