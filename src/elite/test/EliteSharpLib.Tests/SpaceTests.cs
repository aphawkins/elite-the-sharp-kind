// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Fakes;
using Useful.Fakes.Audio;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests;

public class SpaceTests
{
    [Fact]
    public void LaunchPlayerSetsLegalStatusFromContrabandCarried()
    {
        // Arrange: this is the "contraband bug" spot - LegalStatus must pick
        // up IsCarryingContraband() on launch, not just at other checkpoints.
        Space space = CreateSpace(
            out GameState gameState, out _, out _, out Trade trade, out _, out _, out _, out _);
        trade.AddCargo(StockType.Narcotics);

        space.LaunchPlayer();

        Assert.Equal(trade.IsCarryingContraband(), gameState.Cmdr.LegalStatus);
        Assert.NotEqual(0, gameState.Cmdr.LegalStatus);
    }

    [Fact]
    public void LaunchPlayerLeavesLegalStatusCleanWithoutContraband()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out _, out _, out _, out _, out _, out _);

        space.LaunchPlayer();

        Assert.Equal(0, gameState.Cmdr.LegalStatus);
    }

    [Fact]
    public void LaunchPlayerSetsFlightStateAndUndocksPlayer()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out _, out _, out _);
        gameState.IsDocked = true;

        space.LaunchPlayer();

        Assert.Equal(12, ship.Speed);
        Assert.Equal(15, ship.Roll);
        Assert.Equal(0, ship.Climb);
        Assert.False(gameState.IsDocked);
        Assert.NotNull(universe.Planet);

        // Planet plus the newly created station.
        Assert.Equal(2, universe.GetAllObjects().Count());
    }

    [Fact]
    public void DockPlayerDocksResetsShipAndWeapons()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        gameState.IsDocked = false;
        gameState.LaserTemp = 5;
        ship.Speed = 12;
        ship.EcmActive = 3;

        space.DockPlayer();

        Assert.True(gameState.IsDocked);
        Assert.Equal(0, ship.Speed);
        Assert.Equal(0, gameState.LaserTemp);
        Assert.Equal(0, ship.EcmActive);
    }

    [Fact]
    public void JumpWarpReportsMassLockedWhenANonExemptObjectIsPresent()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        AddFarPlanetAndStation(universe, draw, rng);
        IShip trader = new FakeShip(draw, rng) { Type = ShipType.CobraMk3 };
        universe.AddNewShip(trader, new(5000, 0, 5000, 0), Matrix4x4.Identity, 0, 0);

        space.JumpWarp();

        Assert.Equal("Mass Locked", gameState.MessageString);

        // The jump did not proceed - the far planet/station kept their position.
        Assert.Equal(0, universe.Planet!.Location.Z);
    }

    [Fact]
    public void JumpWarpReportsMassLockedWhenThereIsNoPlanet()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out _, out _, out _, out _, out _, out _);

        space.JumpWarp();

        Assert.Equal("Mass Locked", gameState.MessageString);
    }

    [Fact]
    public void JumpWarpMovesEverythingByTheClampedJumpDistance()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out _, out _, out Combat combat, out FakeEliteDraw draw, out RNG rng, out _);
        AddFarPlanetAndStation(universe, draw, rng);
        combat.InBattle = true;

        space.JumpWarp();

        // Planet is 100000 out, station is 150000 out: the raw jump of
        // 100000-75000=25000 is clamped down to the 1024 cap.
        Assert.Equal(-1024, universe.Planet!.Location.Z);
        Assert.Equal(-1024, universe.StationOrSun!.Location.Z);
        Assert.False(combat.InBattle);
    }

    [Fact]
    public void StartHyperspaceDoesNothingWhenAlreadyReady()
    {
        Space space = CreateSpace(
            out _, out _, out _, out _, out _, out _, out _, out _);
        space.IsHyperspaceReady = true;

        space.StartHyperspace();

        Assert.Equal(0, space.HyperCountdown);
        Assert.False(space.HyperGalactic);
    }

    [Fact]
    public void StartHyperspaceDoesNothingWhenDistanceExceedsFuel()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        gameState.DockedPlanet = new() { D = 0 };
        gameState.HyperspacePlanet = new() { D = 20 };
        ship.Fuel = 7;

        space.StartHyperspace();

        Assert.False(space.IsHyperspaceReady);
    }

    [Fact]
    public void StartHyperspaceEngagesWhenFuelIsSufficient()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        gameState.DockedPlanet = new() { D = 0 };
        gameState.HyperspacePlanet = new() { D = 3 };
        ship.Fuel = 7;

        space.StartHyperspace();

        Assert.True(space.IsHyperspaceReady);
        Assert.Equal(15, space.HyperCountdown);
        Assert.False(space.HyperGalactic);
        Assert.NotEmpty(space.HyperName);
    }

    [Fact]
    public void StartGalacticHyperspaceRequiresGalacticHyperdrive()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        ship.HasGalacticHyperdrive = false;

        space.StartGalacticHyperspace();

        Assert.False(space.IsHyperspaceReady);
        Assert.Equal(0, space.HyperCountdown);
    }

    [Fact]
    public void StartGalacticHyperspaceEngagesWhenEquipped()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        ship.HasGalacticHyperdrive = true;

        space.StartGalacticHyperspace();

        Assert.True(space.IsHyperspaceReady);
        Assert.Equal(2, space.HyperCountdown);
        Assert.True(space.HyperGalactic);
    }

    [Fact]
    public void CountdownHyperspaceDecrementsWithoutCompleting()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        ship.HasGalacticHyperdrive = true;
        space.StartGalacticHyperspace();

        space.CountdownHyperspace();

        Assert.Equal(1, space.HyperCountdown);
        Assert.True(space.IsHyperspaceReady);
    }

    [Fact]
    public void CountdownHyperspaceCompletesGalacticHyperspaceAtZero()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out _, out _, out _);
        ship.HasGalacticHyperdrive = true;
        space.StartGalacticHyperspace();

        // HyperCountdown starts at 2: two calls decrement it to zero, the
        // third sees zero and completes the jump.
        space.CountdownHyperspace();
        space.CountdownHyperspace();
        space.CountdownHyperspace();

        Assert.False(space.IsHyperspaceReady);
        Assert.False(space.HyperGalactic);
        Assert.False(ship.HasGalacticHyperdrive);
        Assert.Equal(0, gameState.Cmdr.LegalStatus);
        Assert.Equal(Screen.Hyperspace, gameState.CurrentScreen);
        Assert.NotNull(universe.Planet);
    }

    [Fact]
    public void EngageDockingComputerSetsDockingViewWhenStationPresent()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip station = new FakeShip(draw, rng) { Type = ShipType.Coriolis };
        universe.AddNewShip(station, Vector4.Zero, Matrix4x4.Identity, 0, 0);

        space.EngageDockingComputer();

        Assert.Equal(Screen.Docking, gameState.CurrentScreen);
    }

    [Fact]
    public void EngageDockingComputerDoesNothingWithoutAStation()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out _, out _, out _, out _, out _, out _);

        space.EngageDockingComputer();

        Assert.NotEqual(Screen.Docking, gameState.CurrentScreen);
    }

    [Fact]
    public void UpdateAltitudeStaysSafeInWitchspace()
    {
        Space space = CreateSpace(
            out GameState gameState, out _, out PlayerShip ship, out _, out _, out _, out _, out _);
        gameState.InWitchspace = true;

        space.UpdateAltitude();

        Assert.Equal(255, ship.Altitude);
    }

    [Fact]
    public void UpdateAltitudeStaysSafeWithNoPlanet()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);

        space.UpdateAltitude();

        Assert.Equal(255, ship.Altitude);
    }

    [Fact]
    public void UpdateAltitudeComputesDistanceNearPlanet()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip planet = new FakeShip(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(planet, new(0, 0, 30000, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateAltitude();

        float expected = MathF.Sqrt((30000f / 256 * (30000f / 256)) - 9472);
        Assert.True(MathF.Abs(expected - ship.Altitude) < 0.01f, $"Expected altitude near {expected}, got {ship.Altitude}.");
    }

    [Fact]
    public void UpdateAltitudeTriggersGameOverWhenTooClose()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip planet = new FakeShip(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(planet, new(0, 0, 100, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateAltitude();

        Assert.Equal(0, ship.Altitude);
        Assert.True(gameState.IsGameOver);
    }

    [Fact]
    public void UpdateCabinTempDefaultsToThirtyWithoutStationOrSun()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);

        space.UpdateCabinTemp();

        Assert.Equal(30, ship.CabinTemperature);
    }

    [Fact]
    public void UpdateCabinTempStaysDefaultWhenStationPresent()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip station = new FakeShip(draw, rng) { Type = ShipType.Coriolis };
        universe.AddNewShip(station, Vector4.Zero, Matrix4x4.Identity, 0, 0);

        space.UpdateCabinTemp();

        Assert.Equal(30, ship.CabinTemperature);
    }

    [Fact]
    public void UpdateCabinTempComputesTemperatureNearSun()
    {
        // Z=64000 -> dist = (64000/256)^2 / 256 = 244.14, (int)244 ^ 255 = 11.
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip sun = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(sun, new(0, 0, 64000, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateCabinTemp();

        Assert.Equal(41, ship.CabinTemperature);
    }

    [Fact]
    public void UpdateCabinTempActivatesFuelScoopWhenHotAndEquipped()
    {
        // Z=28672 -> dist = (112)^2 / 256 = 49 exactly, (int)49 ^ 255 = 206,
        // landing the cabin temperature in the fuel-scoop band (>=224, <255).
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip sun = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(sun, new(0, 0, 28672, 0), Matrix4x4.Identity, 0, 0);
        ship.HasFuelScoop = true;
        ship.Speed = 10;
        ship.Fuel = 0;

        space.UpdateCabinTemp();

        Assert.Equal(236, ship.CabinTemperature);
        Assert.Equal(5, ship.Fuel);
        Assert.Equal("Fuel Scoop On", gameState.MessageString);
    }

    [Fact]
    public void UpdateCabinTempDoesNotActivateFuelScoopWithoutEquipment()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip sun = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(sun, new(0, 0, 28672, 0), Matrix4x4.Identity, 0, 0);
        ship.HasFuelScoop = false;
        ship.Fuel = 0;

        space.UpdateCabinTemp();

        Assert.Equal(236, ship.CabinTemperature);
        Assert.Equal(0, ship.Fuel);
    }

    [Fact]
    public void UpdateCabinTempTriggersGameOverWhenTooHot()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip sun = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(sun, new(0, 0, 256, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateCabinTemp();

        Assert.Equal(255, ship.CabinTemperature);
        Assert.True(gameState.IsGameOver);
    }

    private static void AddFarPlanetAndStation(Universe universe, FakeEliteDraw draw, RNG rng)
    {
        IShip planet = new FakeShip(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(planet, new(100000, 0, 0, 0), Matrix4x4.Identity, 0, 0);

        IShip station = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(station, new(0, 150000, 0, 0), Matrix4x4.Identity, 0, 0);
    }

    private static Space CreateSpace(
        out GameState gameState,
        out Universe universe,
        out PlayerShip ship,
        out Trade trade,
        out Combat combat,
        out FakeEliteDraw draw,
        out RNG rng,
        out FakeRandomSource randomSource)
    {
        ScreenManager<Screen, IView> views = new(new FakeKeyboard());
        views.Add(Screen.Docking, new FakeView());
        views.Add(Screen.GameOver, new FakeView());
        views.Add(Screen.Hyperspace, new FakeView());
        gameState = new(views);
        ship = new PlayerShip();
        trade = new Trade(gameState, ship);
        draw = new FakeEliteDraw();
        randomSource = new FakeRandomSource();
        rng = new(randomSource);
        FakeShipFactory shipFactory = new(draw, rng);
        universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        combat = new Combat(gameState, audio, ship, trade, pilot, universe, draw, shipFactory, rng);
        PlanetController planet = new(gameState);
        Stars stars = new(gameState, draw, ship, rng);

        return new Space(gameState, audio, pilot, combat, trade, ship, planet, stars, universe, draw, rng);
    }
}
