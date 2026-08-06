// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Missions;
using EliteSharpLib.Planets;
using EliteSharpLib.Ships;
using EliteSharpLib.Suns;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Fakes;
using Useful.Fakes.Audio;
using Useful.Fakes.Input;
using Useful.Graphics.Rendering;

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
        gameState.LaserTemp = 5 * GameState.LaserTempStep;
        ship.Speed = 12;
        ship.EcmActive = 3;

        space.DockPlayer();

        Assert.True(gameState.IsDocked);
        Assert.Equal(0, ship.Speed);
        Assert.Equal(GameState.LaserTempMin, gameState.LaserTemp);
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

        Assert.Equal(PlayerShip.AltitudeMax, ship.Altitude);
    }

    [Fact]
    public void UpdateAltitudeStaysSafeWithNoPlanet()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);

        space.UpdateAltitude();

        Assert.Equal(PlayerShip.AltitudeMax, ship.Altitude);
    }

    [Fact]
    public void UpdateAltitudeComputesDistanceNearPlanet()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip planet = new FakeShip(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(planet, new(0, 0, 30000, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateAltitude();

        float expected = MathF.Sqrt((30000f / 256 * (30000f / 256)) - 9472) * PlayerShip.AltitudeStep;
        Assert.True(MathF.Abs(expected - ship.Altitude) < 0.0001f, $"Expected altitude near {expected}, got {ship.Altitude}.");
    }

    [Fact]
    public void AnEnergyBombKillsAConstrictor()
    {
        // Arrange: the original's bomb-kill sweep excludes only the space
        // station and already-exploding ships - no ship-type check at all,
        // so the Constrictor's "only military lasers penetrate" gimmick
        // doesn't save it from a bomb.
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        gameState.DetonateBomb = true;
        FakeShip constrictor = new(draw, rng) { Type = ShipType.Constrictor };
        universe.AddNewShip(constrictor, new(0, 0, 500, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateUniverse();

        Assert.True(constrictor.Flags.HasFlag(ShipProperties.Dead));
    }

    [Fact]
    public void RollingAndClimbingDoesNotRotateTheSunsOwnOrientation()
    {
        // Arrange: original MV45 - the sun returns before rotating its own
        // orientation vectors or spinning, "we don't need to rotate the sun
        // around its origin." A planet, by contrast, still rotates.
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        ship.Roll = 30;
        ship.Climb = 20;
        FakeShip sun = new(draw, rng) { Type = ShipType.Sun, Rotmat = Matrix4x4.Identity };
        universe.AddNewShip(sun, new(0, 0, 500, 0), Matrix4x4.Identity, 0, 0);
        FakeShip planet = new(draw, rng) { Type = ShipType.Planet, Rotmat = Matrix4x4.Identity };
        universe.AddNewShip(planet, new(1000, 0, 500, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateUniverse();

        Assert.Equal(Matrix4x4.Identity, sun.Rotmat);
        Assert.NotEqual(Matrix4x4.Identity, planet.Rotmat);
    }

    [Fact]
    public void UpdateAltitudeTriggersGameOverWhenTooClose()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip planet = new FakeShip(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(planet, new(0, 0, 100, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateAltitude();

        Assert.Equal(PlayerShip.AltitudeMin, ship.Altitude);
        Assert.True(gameState.IsGameOver);
    }

    [Fact]
    public void UpdateCabinTempDefaultsToThirtyWithoutStationOrSun()
    {
        Space space = CreateSpace(
            out _, out _, out PlayerShip ship, out _, out _, out _, out _, out _);

        space.UpdateCabinTemp();

        Assert.Equal(PlayerShip.AmbientTemperature, ship.CabinTemperature);
    }

    [Fact]
    public void UpdateCabinTempStaysDefaultWhenStationPresent()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip station = new FakeShip(draw, rng) { Type = ShipType.Coriolis };
        universe.AddNewShip(station, Vector4.Zero, Matrix4x4.Identity, 0, 0);

        space.UpdateCabinTemp();

        Assert.Equal(PlayerShip.AmbientTemperature, ship.CabinTemperature);
    }

    [Fact]
    public void UpdateCabinTempComputesTemperatureNearSun()
    {
        // Z=64000 -> dist = (64000/256)^2 / 256 = 244.14, inverted to 256-244 = 12.
        Space space = CreateSpace(
            out _, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip sun = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(sun, new(0, 0, 64000, 0), Matrix4x4.Identity, 0, 0);

        space.UpdateCabinTemp();

        Assert.Equal((12 * PlayerShip.TemperatureStep) + PlayerShip.AmbientTemperature, ship.CabinTemperature);
    }

    [Fact]
    public void UpdateCabinTempActivatesFuelScoopWhenHotAndEquipped()
    {
        // Z=28672 -> dist = (112)^2 / 256 = 49 exactly, inverted to 256-49 = 207,
        // landing the cabin temperature in the fuel-scoop band (>=224, <256).
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out PlayerShip ship, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip sun = new FakeShip(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(sun, new(0, 0, 28672, 0), Matrix4x4.Identity, 0, 0);
        ship.HasFuelScoop = true;
        ship.Speed = 10;
        ship.Fuel = 0;

        space.UpdateCabinTemp();

        Assert.Equal((207 * PlayerShip.TemperatureStep) + PlayerShip.AmbientTemperature, ship.CabinTemperature);
        Assert.Equal(0.125f, ship.Fuel);
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

        Assert.Equal((207 * PlayerShip.TemperatureStep) + PlayerShip.AmbientTemperature, ship.CabinTemperature);
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

        Assert.Equal(PlayerShip.TemperatureMax, ship.CabinTemperature);
        Assert.True(gameState.IsGameOver);
    }

    [Fact]
    public void RefreshPlanetStyleRebuildsThePlanetInPlace()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        FakeShip oldPlanet = new(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(oldPlanet, new(0, 0, 65536, 0), Matrix4x4.Identity, 0, 0);
        gameState.Config.Engine.Graphics.GraphicStyle = GraphicStyle.Wireframe;

        space.RefreshPlanetStyle();

        // Only the outlined style spins, so a turning planet is a wireframe one.
        // There is one Planet class now: the style lives in its renderer.
        Assert.NotNull(universe.Planet);
        Assert.Equal(127, universe.Planet.RotX);
        Assert.Equal(oldPlanet.Location, universe.Planet.Location);
        Assert.Equal(oldPlanet.Rotmat, universe.Planet.Rotmat);

        // The replacement is a swap, not an addition.
        Assert.Single(universe.GetAllObjects());
    }

    [Fact]
    public void RefreshPlanetStyleDoesNothingWithoutAPlanet()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out _, out _, out _, out _, out _, out _);

        space.RefreshPlanetStyle();

        Assert.Null(universe.Planet);
    }

    [Fact]
    public void RefreshSunStyleRebuildsTheSunInPlace()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        FakeShip oldSun = new(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(oldSun, new(0, 0, 64000, 0), Matrix4x4.Identity, 0, 0);
        gameState.Config.Game.SunStyle = SunType.Solid;

        space.RefreshSunStyle();

        // One Sun class now: which style it is lives in its renderer, and what
        // each style draws is covered by the renderer tests.
        Assert.IsType<Sun>(universe.StationOrSun);
        Assert.Equal(oldSun.Location, universe.StationOrSun.Location);
        Assert.Single(universe.GetAllObjects());
    }

    [Fact]
    public void RefreshSunStyleGivesAWireframeWorldAWireframeSunWhateverTheSunStyle()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        FakeShip oldSun = new(draw, rng) { Type = ShipType.Sun };
        universe.AddNewShip(oldSun, new(0, 0, 64000, 0), Matrix4x4.Identity, 0, 0);
        gameState.Config.Engine.Graphics.GraphicStyle = GraphicStyle.Wireframe;
        gameState.Config.Game.SunStyle = SunType.Gradient;

        space.RefreshSunStyle();

        Assert.IsType<Sun>(universe.StationOrSun);
    }

    [Fact]
    public void RefreshPlanetStyleGivesAWireframeWorldAWireframePlanetWhateverThePlanetStyle()
    {
        Space space = CreateSpace(
            out GameState gameState, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        FakeShip oldPlanet = new(draw, rng) { Type = ShipType.Planet };
        universe.AddNewShip(oldPlanet, new(0, 0, 65536, 0), Matrix4x4.Identity, 0, 0);
        gameState.Config.Engine.Graphics.GraphicStyle = GraphicStyle.Wireframe;
        gameState.Config.Game.PlanetStyle = PlanetType.Fractal;

        space.RefreshPlanetStyle();

        // Only the outlined style spins, so a turning planet is a wireframe one.
        // There is one Planet class now: the style lives in its renderer.
        Assert.NotNull(universe.Planet);
        Assert.Equal(127, universe.Planet.RotX);
    }

    [Fact]
    public void RefreshSunStyleLeavesAStationAlone()
    {
        Space space = CreateSpace(
            out _, out Universe universe, out _, out _, out _, out FakeEliteDraw draw, out RNG rng, out _);
        IShip station = new FakeShip(draw, rng) { Type = ShipType.Coriolis, Flags = ShipProperties.Station };
        universe.AddNewShip(station, Vector4.Zero, Matrix4x4.Identity, 0, 0);

        space.RefreshSunStyle();

        Assert.Same(station, universe.StationOrSun);
    }

    [Fact]
    public void CompletingHyperspaceMisjumpsOnARandomByteOf253()
    {
        // Original TT18: a misjump triggers when the random byte is >= 253
        // (253, 254 or 255). 253 itself was previously being treated as a
        // safe jump.
        Space space = CreateSpace(
            out GameState gameState, out _, out PlayerShip ship, out _, out _, out _, out _, out FakeRandomSource randomSource);
        ship.Fuel = 7;
        gameState.DockedPlanet.D = 0;
        gameState.HyperspacePlanet.D = 10;
        randomSource.RandomValue = 253;

        space.StartHyperspace();
        for (int i = 0; i < 16; i++)
        {
            space.CountdownHyperspace();
        }

        Assert.True(gameState.InWitchspace);
    }

    [Fact]
    public void MisjumpingIntoWitchspaceAlwaysSpawnsExactlyFourThargoids()
    {
        // Original MJP1: GTHG loops until MANY+THG exceeds 3, always ending
        // with exactly 4 Thargoids - not a random 1-4. A random byte of 253
        // also clears the >64 escort threshold in CreateThargoid, so each
        // Thargoid brings its Tharglet companion too.
        Space space = CreateSpace(
            out GameState gameState,
            out Universe universe,
            out PlayerShip ship,
            out _,
            out _,
            out _,
            out _,
            out FakeRandomSource randomSource);
        ship.Fuel = 7;
        gameState.DockedPlanet.D = 0;
        gameState.HyperspacePlanet.D = 10;
        randomSource.RandomValue = 253;

        space.StartHyperspace();
        for (int i = 0; i < 16; i++)
        {
            space.CountdownHyperspace();
        }

        Assert.Equal(8, universe.GetAllObjects().Count());
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
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        views.Add(Screen.Docking, new FakeView());
        views.Add(Screen.GameOver, new FakeView());
        views.Add(Screen.Hyperspace, new FakeView());
        gameState = new(views, TestMissions.Registry());
        ship = new PlayerShip();
        trade = new Trade(gameState, ship);
        draw = new FakeEliteDraw();
        randomSource = new FakeRandomSource();
        rng = new(randomSource);
        FakeShipFactory shipFactory = new(draw, rng);
        universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        MissionRunner missions = TestMissions.Runner(gameState, ship, trade);

        combat = new Combat(
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
        PlanetController planet = new(gameState);
        Stars stars = new(gameState, draw, ship, new SixteenBitRendition().CreateStarfieldRenderer(draw), rng);

        return new Space(
            gameState,
            audio,
            pilot,
            combat,
            trade,
            ship,
            planet,
            stars,
            universe,
            draw,
            new SixteenBitRendition(),
            rng);
    }
}
