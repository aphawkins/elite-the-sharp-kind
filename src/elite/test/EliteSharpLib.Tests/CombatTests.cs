// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using System.Reflection;
using EliteSharp.Abstractions.Ships;
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

namespace EliteSharpLib.Tests;

public class CombatTests
{
    [Fact]
    public void CreateThargoidLaunchesTharglet()
    {
        // Arrange: the 1-in-256-ish escort roll (RNG.Random(256) > 64) is
        // forced deterministically instead of hunting for a seed that hits it.
        Combat combat = CreateCombat(out Universe universe, out _, out _, out _, randomValue: 254);

        // Act
        combat.CreateThargoid();

        // Assert: the Thargoid plus its Tharglet escort are both in the universe.
        Assert.Equal(2, universe.GetAllObjects().Count());
    }

    [Fact]
    public void CreateThargoidDoesNotLaunchThargletBelowThreshold()
    {
        // Arrange: force the same roll to miss the threshold.
        Combat combat = CreateCombat(out Universe universe, out _, out _, out _, randomValue: 0);

        // Act
        combat.CreateThargoid();

        // Assert: only the Thargoid itself is in the universe.
        Assert.Single(universe.GetAllObjects());
    }

    [Fact]
    public void ScoopingWithAFullHoldDestroysTheCanisterWithoutDamage()
    {
        // Arrange: original MA59 - scooping fails because the hold is full,
        // which only plays a destruction sound, no OOPS damage call.
        Combat combat = CreateCombat(out _, out PlayerShip ship, out _, out _, randomValue: 0);
        ship.HasFuelScoop = true;
        ship.CargoCapacity = 0;
        ship.ShieldFront = PlayerShip.ShieldMax;
        ship.ShieldRear = PlayerShip.ShieldMax;
        FakeShip canister = new(new FakeEliteDraw(), new(new FakeRandomSource()))
        {
            Type = ShipType.Cargo,
            Location = new(0, -100, 500, 0),
        };

        combat.ScoopItem(canister);

        Assert.Equal(PlayerShip.ShieldMax, ship.ShieldFront);
        Assert.Equal(PlayerShip.ShieldMax, ship.ShieldRear);
        Assert.True(canister.Flags.HasFlag(ShipProperties.Dead));
    }

    [Fact]
    public void ScoopingWithNoFuelScoopFittedTakesCollisionDamage()
    {
        // Arrange: original MA58 - can't scoop at all, so this is a genuine
        // collision and takes full OOPS damage.
        Combat combat = CreateCombat(out _, out PlayerShip ship, out _, out _, randomValue: 0);
        ship.HasFuelScoop = false;
        ship.CargoCapacity = 100;
        ship.ShieldFront = PlayerShip.ShieldMax;
        FakeShip canister = new(new FakeEliteDraw(), new(new FakeRandomSource()))
        {
            Type = ShipType.Cargo,
            Location = new(0, -100, 500, 0),
        };

        combat.ScoopItem(canister);

        Assert.True(ship.ShieldFront < PlayerShip.ShieldMax);
    }

    [Fact]
    public void DestroyingAnAsteroidWithAPulseLaserYieldsAlloyAndCargoButNoSplinters()
    {
        // Arrange: original CMP #Mlas only spawns splinters when the killing
        // laser is exactly the mining laser - Pulse-laser kills get none -
        // but every kill, asteroids included, still falls through to spawn
        // alloy plates and cargo canisters.
        Combat combat = CreateCombat(out Universe universe, out _, out _, out _, randomValue: 2);
        SetLaserType(combat, LaserType.Pulse);
        FakeShip asteroid = new(new FakeEliteDraw(), new(new FakeRandomSource())) { Type = ShipType.Asteroid, LootMax = 15 };

        InvokeDestroyTarget(combat, asteroid);

        // Alloy (2) and cargo (2) still spawn; no rock splinters.
        Assert.Equal(4, universe.GetAllObjects().Count());
    }

    [Fact]
    public void DestroyingAnAsteroidWithAMiningLaserYieldsSplintersAlloyAndCargo()
    {
        // Arrange: a mining-laser kill gets splinters in addition to the
        // alloy/cargo every kill yields, not instead of it.
        Combat combat = CreateCombat(out Universe universe, out _, out _, out _, randomValue: 2);
        SetLaserType(combat, LaserType.Mining);
        FakeShip asteroid = new(new FakeEliteDraw(), new(new FakeRandomSource())) { Type = ShipType.Asteroid, LootMax = 15 };

        InvokeDestroyTarget(combat, asteroid);

        // Splinters (2) plus alloy (2) plus cargo (2).
        Assert.Equal(6, universe.GetAllObjects().Count());
    }

    [Fact]
    public void PoliceIgnoreLegalStatusWithNoPoliceNearby()
    {
        // Arrange: original LDX MANY+COPS; BEQ P%+5 skips ORing in our legal
        // status when there are no cops in the bubble yet - they haven't
        // scanned us, so a bad reputation alone shouldn't summon them.
        Combat combat = CreateCombat(out Universe universe, out _, out _, out GameState gameState, randomValue: 50);
        gameState.Cmdr.LegalStatus = 200;

        InvokeCheckForPolice(combat);

        Assert.Empty(universe.GetAllObjects());
    }

    [Fact]
    public void PoliceFactorInLegalStatusWhenAlreadyPresent()
    {
        // Arrange: with cops already in the bubble, our legal status is ORed
        // into the spawn chance - they've almost certainly scanned us.
        Combat combat = CreateCombat(out Universe universe, out _, out _, out GameState gameState, randomValue: 50);
        gameState.Cmdr.LegalStatus = 200;
        FakeShip existingPolice = new(new FakeEliteDraw(), new(new FakeRandomSource())) { Type = ShipType.Viper };
        universe.AddNewShip(existingPolice, default, Matrix4x4.Identity, 0, 0);

        InvokeCheckForPolice(combat);

        // The existing Viper plus a newly spawned police ship.
        Assert.Equal(2, universe.GetAllObjects().Count());
    }

    [Fact]
    public void ABountyHunterLoneWolfIsNotHostileBelowLegalStatus40()
    {
        // Arrange: original TACTICS part 3 - a bounty hunter (NEWB bit 1)
        // only turns hostile once FIST >= 40, an "Offender" but not yet a
        // "Fugitive."
        Combat combat = CreateCombat(out Universe universe, out _, out _, out GameState gameState, randomValue: 0);
        gameState.Cmdr.LegalStatus = 39;

        InvokeCreateLoneWolf(combat);

        Assert.False(Assert.Single(universe.GetAllObjects()).Flags.HasFlag(ShipProperties.Angry));
    }

    [Fact]
    public void ABountyHunterLoneWolfIsHostileAtLegalStatus40()
    {
        Combat combat = CreateCombat(out Universe universe, out _, out _, out GameState gameState, randomValue: 0);
        gameState.Cmdr.LegalStatus = 40;

        InvokeCreateLoneWolf(combat);

        Assert.True(Assert.Single(universe.GetAllObjects()).Flags.HasFlag(ShipProperties.Angry));
    }

    private static void InvokeCreateLoneWolf(Combat combat)
    {
        MethodInfo method = typeof(Combat).GetMethod("CreateLoneWolf", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMethodException(nameof(Combat), "CreateLoneWolf");
        method.Invoke(combat, null);
    }

    private static void InvokeCheckForPolice(Combat combat)
    {
        MethodInfo method = typeof(Combat).GetMethod("CheckForPolice", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMethodException(nameof(Combat), "CheckForPolice");
        method.Invoke(combat, null);
    }

    private static void SetLaserType(Combat combat, LaserType laserType)
    {
        FieldInfo field = typeof(Combat).GetField("_laserType", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(nameof(Combat), "_laserType");
        field.SetValue(combat, laserType);
    }

    private static void InvokeDestroyTarget(Combat combat, IShip obj)
    {
        MethodInfo method = typeof(Combat).GetMethod("DestroyTarget", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMethodException(nameof(Combat), "DestroyTarget");
        method.Invoke(combat, [obj]);
    }

    private static Combat CreateCombat(
        out Universe universe, out PlayerShip ship, out Trade trade, out GameState gameState, int randomValue)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        gameState = new(views, TestMissions.Registry());
        ship = new PlayerShip();
        trade = new Trade(gameState, ship);
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource { RandomValue = randomValue });
        FakeShipFactory shipFactory = new(draw, rng);
        universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);

        MissionRunner missions = TestMissions.Runner(gameState, ship, trade);

        return new Combat(
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
    }
}
