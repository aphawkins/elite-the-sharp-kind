// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
        Combat combat = CreateCombat(out Universe universe, out _, out _, randomValue: 254);

        // Act
        combat.CreateThargoid();

        // Assert: the Thargoid plus its Tharglet escort are both in the universe.
        Assert.Equal(2, universe.GetAllObjects().Count());
    }

    [Fact]
    public void CreateThargoidDoesNotLaunchThargletBelowThreshold()
    {
        // Arrange: force the same roll to miss the threshold.
        Combat combat = CreateCombat(out Universe universe, out _, out _, randomValue: 0);

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
        Combat combat = CreateCombat(out _, out PlayerShip ship, out _, randomValue: 0);
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
        Combat combat = CreateCombat(out _, out PlayerShip ship, out _, randomValue: 0);
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
        Combat combat = CreateCombat(out Universe universe, out _, out _, randomValue: 2);
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
        Combat combat = CreateCombat(out Universe universe, out _, out _, randomValue: 2);
        SetLaserType(combat, LaserType.Mining);
        FakeShip asteroid = new(new FakeEliteDraw(), new(new FakeRandomSource())) { Type = ShipType.Asteroid, LootMax = 15 };

        InvokeDestroyTarget(combat, asteroid);

        // Splinters (2) plus alloy (2) plus cargo (2).
        Assert.Equal(6, universe.GetAllObjects().Count());
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

    private static Combat CreateCombat(out Universe universe, out PlayerShip ship, out Trade trade, int randomValue)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
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
