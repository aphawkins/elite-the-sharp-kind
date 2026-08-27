// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using SharpKind.Abstraction;
using SharpKind.Fakes.Input;

namespace EliteSharpLib.Tests;

public class TradeTests
{
    [Fact]
    public void IsCarryingContrabandCountsSlavesAndNarcoticsOnce()
    {
        // Arrange
        Trade trade = CreateTrade();
        trade["Slaves"].CurrentCargo = 2;
        trade["Narcotics"].CurrentCargo = 3;
        trade["Firearms"].CurrentCargo = 1;

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
        trade["Food"].CurrentCargo = 10;

        // Act
        int contraband = trade.IsCarryingContraband();

        // Assert
        Assert.Equal(0, contraband);
    }

    private static Trade CreateTrade()
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
        PlayerShip ship = new(gameState);
        return TestGoods.Trade(gameState, ship);
    }
}
