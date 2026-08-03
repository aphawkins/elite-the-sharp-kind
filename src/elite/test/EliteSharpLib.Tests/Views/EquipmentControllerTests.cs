// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Fakes;
using Useful.Fakes.Audio;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// The equip-ship list's visibility and cursor, with no renderer involved:
// collapsed laser categories never reach the model at all, rather than
// reaching it hidden.
public class EquipmentControllerTests
{
    [Fact]
    public void ResetShowsOnlyTopLevelRows()
    {
        EquipmentController controller = CreateController(out GameState gameState, out _);
        gameState.CurrentPlanetData.TechLevel = 10;

        controller.Reset();

        EquipmentModel model = controller.BuildModel();
        Assert.All(model.Rows, row => Assert.False(row.IsIndented));
        Assert.Contains(model.Rows, row => row.Name == "Pulse Laser");
    }

    [Fact]
    public void BuyingALaserCategoryRevealsItsMountChoices()
    {
        EquipmentController controller = CreateController(out GameState gameState, out _);
        gameState.CurrentPlanetData.TechLevel = 10;
        controller.Reset();

        // The ten top-level rows (Fuel..Galactic Hyperdrive) precede Pulse
        // Laser in the stock array, so ten SelectNext calls reach it.
        for (int i = 0; i < 10; i++)
        {
            controller.SelectNext();
        }

        controller.Buy();

        // Buying a category expands it into its mount choices, but ListPrices
        // resets the cursor to row zero as a side effect - a pre-existing
        // quirk, preserved rather than fixed here.
        EquipmentModel model = controller.BuildModel();
        Assert.Contains(model.Rows, row => row.Name == "Front" && row.IsIndented);
        Assert.True(model.Rows[0].IsHighlighted);
    }

    [Fact]
    public void APriceThatTruncatesToZeroIsNotShown()
    {
        EquipmentController controller = CreateController(out GameState gameState, out PlayerShip ship);
        gameState.CurrentPlanetData.TechLevel = 10;
        ship.Fuel = 6.9f; // (7 - 6.9) * 2 = 0.2, truncates to zero.

        controller.Reset();

        Assert.Equal(string.Empty, controller.BuildModel().Rows[0].Price);
    }

    [Fact]
    public void ANonZeroPriceIsFormattedToOneDecimalPlace()
    {
        EquipmentController controller = CreateController(out GameState gameState, out PlayerShip ship);
        gameState.CurrentPlanetData.TechLevel = 10;
        ship.Fuel = 0;

        controller.Reset();

        Assert.Equal("14.0", controller.BuildModel().Rows[0].Price);
    }

    private static EquipmentController CreateController(out GameState gameState, out PlayerShip ship)
    {
        FakeKeyboard keyboard = new();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        gameState = new(views, TestMissions.Registry());
        ship = new PlayerShip();
        Trade trade = new(gameState, ship);
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource());
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        MissionRunner missions = TestMissions.Runner(gameState, ship, trade);

        Combat combat = new(
            gameState,
            audio,
            ship,
            trade,
            pilot,
            universe,
            draw,
            new SixteenBitRendition(),
            shipFactory,
            rng,
            missions);
        ScannerController scanner = new(gameState, ship, universe, combat, new NothingScannerView());

        return new EquipmentController(gameState, keyboard, ship, trade, scanner, new FakeEquipmentView());
    }

    // Buying fuel or a missile refreshes the HUD; what it draws is not under
    // test here.
    private sealed class NothingScannerView : IView<ScannerModel>
    {
        public void Draw(ScannerModel model)
        {
        }
    }

    private sealed class FakeEquipmentView : IView<EquipmentModel>
    {
        public void Draw(EquipmentModel model)
        {
            // Drawing is not under test here.
        }
    }
}
