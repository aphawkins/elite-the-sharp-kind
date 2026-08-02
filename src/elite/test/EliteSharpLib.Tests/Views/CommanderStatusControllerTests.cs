// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Equipment;
using EliteSharpLib.Fakes;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// The commander status screen's formatting, with no renderer in sight - the
// point of the screen producing a model rather than drawing directly.
public class CommanderStatusControllerTests
{
    [Theory]
    [InlineData(0, "Harmless")]
    [InlineData(0x0008, "Mostly Harmless")]
    [InlineData(0x0020, "Average")]
    [InlineData(0x1900, "---- E L I T E ----")]
    [InlineData(0x7FFF, "---- E L I T E ----")]
    public void RatingIsTheHighestBandTheScoreReaches(int score, string expected)
    {
        CommanderStatusController controller = CreateController(out GameState gameState, out _);
        gameState.Cmdr.Score = score;

        Assert.Equal(expected, controller.BuildModel().Rating);
    }

    [Fact]
    public void ConditionIsDockedWhileDocked()
    {
        CommanderStatusController controller = CreateController(out GameState gameState, out _);
        gameState.IsDocked = true;

        Assert.Equal("Docked", controller.BuildModel().Condition);
    }

    [Fact]
    public void ConditionIsGreenInEmptySpace()
    {
        CommanderStatusController controller = CreateController(out GameState gameState, out _);
        gameState.IsDocked = false;

        Assert.Equal("Green", controller.BuildModel().Condition);
    }

    [Theory]
    [InlineData(0, "Clean")]
    [InlineData(50, "Offender")]
    [InlineData(51, "Fugitive")]
    public void LegalStatusBandsOnTheCommandersRecord(int legalStatus, string expected)
    {
        CommanderStatusController controller = CreateController(out GameState gameState, out _);
        gameState.Cmdr.LegalStatus = legalStatus;

        Assert.Equal(expected, controller.BuildModel().LegalStatus);
    }

    [Fact]
    public void PresentSystemIsBlankInWitchspace()
    {
        CommanderStatusController controller = CreateController(out GameState gameState, out _);
        gameState.InWitchspace = true;

        Assert.Equal(string.Empty, controller.BuildModel().PresentSystem);
    }

    [Fact]
    public void EquipmentListsOnlyWhatIsFitted()
    {
        CommanderStatusController controller = CreateController(out _, out PlayerShip ship);
        ship.HasECM = true;
        ship.HasFuelScoop = true;
        ship.EnergyUnit = EnergyUnit.Naval;

        IReadOnlyList<string> equipment = controller.BuildModel().Equipment;

        Assert.Contains("E.C.M. System", equipment);
        Assert.Contains("Fuel Scoops", equipment);
        Assert.Contains("Naval Energy Unit", equipment);
        Assert.DoesNotContain("Energy Bomb", equipment);
        Assert.DoesNotContain("Escape Capsule", equipment);
    }

    [Fact]
    public void FuelAndCashCarryTheirUnits()
    {
        CommanderStatusController controller = CreateController(out _, out PlayerShip ship);
        ship.Fuel = 6.5f;

        CommanderStatusModel model = controller.BuildModel();

        Assert.Equal("6.5 Light Years", model.Fuel);
        Assert.EndsWith("Credits", model.Cash, StringComparison.Ordinal);
    }

    private static CommanderStatusController CreateController(out GameState gameState, out PlayerShip ship)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        gameState = new(views, ClassicMissions.Registry());
        ship = new PlayerShip();
        Trade trade = new(gameState, ship);
        RNG rng = new(new FakeRandomSource());
        FakeEliteDraw draw = new();

        return new CommanderStatusController(
            gameState,
            ship,
            trade,
            new PlanetController(gameState),
            new Universe(new FakeShipFactory(draw, rng), rng),
            new FakeCommanderStatusView());
    }

    private sealed class FakeCommanderStatusView : IView<CommanderStatusModel>
    {
        public void Draw(CommanderStatusModel model)
        {
            // Drawing is not under test here.
        }
    }
}
