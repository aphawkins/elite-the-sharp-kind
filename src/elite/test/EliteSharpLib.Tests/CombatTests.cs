// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
