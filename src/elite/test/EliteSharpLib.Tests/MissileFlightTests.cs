// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Conflict;
using EliteSharpLib.Ships;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Tests;

// Arming, locking and hitting are three separate steps that only meet in the
// real update loop - Space moves the ships, Combat.Tactics steers the missile,
// and the lock happens off the drawn (view-space) position. These drive the
// whole loop rather than Combat alone, so a break anywhere along it shows up.
public class MissileFlightTests
{
    // Flying into the intro and out of the station, which every test here
    // starts from.
    private static readonly KeyScriptEvent[] s_launch =
    [
        new(1, ConsoleKey.N, KeyScriptAction.Tap),
        new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),
        new(3, ConsoleKey.F1, KeyScriptAction.Tap),
    ];

    [Fact]
    public void AnArmedMissileLocksOntoAShipInTheCrosshairs()
    {
        using HeadlessGameHarness harness = new();
        harness.Run(4, s_launch);

        Universe universe = harness.Resolve<Universe>();
        Combat combat = harness.Resolve<Combat>();

        IShip enemy = harness.Resolve<IShipFactory>().CreateShip("Sidewinder");
        Assert.True(universe.AddNewShip(enemy, new(0, 0, 3000, 0), VectorMaths.GetLeftHandedBasisMatrix, 0, 0));

        combat.ArmMissile();
        Assert.True(combat.IsMissileArmed);

        // Dead ahead, so the lock is the loop's own doing - no test reaches
        // in to set the target.
        for (int tick = 0; tick < 20 && combat.MissileTarget is null; tick++)
        {
            harness.Step([]);
        }

        Assert.True(combat.MissileTarget is not null, $"never locked; enemy at {enemy.Location}");
    }

    [Fact]
    public void AFiredMissileReachesItsTargetAndDetonates()
    {
        using HeadlessGameHarness harness = new();
        harness.Run(4, s_launch);

        Universe universe = harness.Resolve<Universe>();
        Combat combat = harness.Resolve<Combat>();

        IShip enemy = harness.Resolve<IShipFactory>().CreateShip("Sidewinder");
        Assert.True(universe.AddNewShip(enemy, new(0, 0, 4000, 0), VectorMaths.GetLeftHandedBasisMatrix, 0, 0));

        combat.ArmMissile();
        combat.CheckTarget(enemy, enemy);
        Assert.NotNull(combat.MissileTarget);

        combat.FireMissile();

        IShip? missile = universe.GetAllObjects().OfType<IShip>().FirstOrDefault(o => o.Type == ShipType.Missile);
        Assert.NotNull(missile);

        float closest = float.MaxValue;
        for (int tick = 0; tick < 200 && !enemy.Flags.HasFlag(ShipProperties.Dead); tick++)
        {
            harness.Step([]);
            closest = MathF.Min(closest, (missile.Location - enemy.Location).Length());
        }

        Assert.True(
            enemy.Flags.HasFlag(ShipProperties.Dead),
            $"missile never detonated; closest approach was {closest}, missile at {missile.Location}");
    }

    // Close enough is a box, not a sphere: each axis within 256, which the
    // 6502 gets by ORing the three high bytes and the New Kind spells out.
    // (200, 200, 200) is 346 away, so a distance test would have let this one
    // sail through - which is what the port used to do.
    [Fact]
    public void AMissileDetonatesInTheCornersOfItsBoxNotJustInsideASphere()
    {
        using HeadlessGameHarness harness = new();
        harness.Run(4, s_launch);

        Universe universe = harness.Resolve<Universe>();
        Combat combat = harness.Resolve<Combat>();

        IShip enemy = harness.Resolve<IShipFactory>().CreateShip("Sidewinder");
        Assert.True(universe.AddNewShip(enemy, new(0, 0, 3000, 0), VectorMaths.GetLeftHandedBasisMatrix, 0, 0));

        combat.ArmMissile();
        combat.CheckTarget(enemy, enemy);
        Assert.NotNull(combat.MissileTarget);
        combat.FireMissile();

        IShip? missile = universe.GetAllObjects().OfType<IShip>().FirstOrDefault(o => o.Type == ShipType.Missile);
        Assert.NotNull(missile);

        // Park the pair a box-corner apart and let one tick's tactics run.
        missile.Velocity = 0;
        enemy.Location = missile.Location + new Vector4(200, 200, 200, 0);
        harness.Step([]);

        Assert.True(enemy.Flags.HasFlag(ShipProperties.Dead));
    }
}
