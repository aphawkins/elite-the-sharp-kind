// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using EliteSharpLib.Views;

namespace EliteSharpLib.Tests;

/// <summary>
/// The same stretch of game played at two different update rates.
/// </summary>
/// <remarks>
/// This is what the whole frame-rate rework was for, and the only test that
/// puts the pieces together: the housekeeping clock, the motion, the
/// animations and the AI pacing all converted separately, each proved
/// unchanged at the game's own 13.5Hz. Here the game is run at 60Hz for the
/// same number of seconds and asked to be in the same place. The runs
/// themselves are in <see cref="FrameRateRuns"/>, played once for the class.
/// </remarks>
[Trait("Level", "Integration")]
public class FrameRateIndependenceTests(FrameRateRuns runs) : IClassFixture<FrameRateRuns>
{
    [Fact]
    public void TheHousekeepingTakesTheSameStepsInTheSameTime()
    {
        // Within one step. The count is driven by elapsed time and the clock
        // keeps its unspent remainder, so 300 updates at 13.5Hz and 1333 at
        // 60Hz are the same 22 seconds and buy the same 276-odd steps; the
        // one that can differ is whichever the truncation of 1333 leaves
        // half-finished.
        int slow = runs.SlowFlight.Harness.Game.State.MCount;
        int fast = runs.FastFlight.Harness.Game.State.MCount;

        Assert.InRange(fast, slow - 1, slow + 1);
    }

    [Fact]
    public void TheGameEndsUpOnTheSameScreenHavingLaunched()
    {
        Assert.Equal(Screen.FrontView, runs.SlowFlight.Harness.Game.State.CurrentScreen);
        Assert.Equal(Screen.FrontView, runs.FastFlight.Harness.Game.State.CurrentScreen);
        Assert.False(runs.SlowFlight.Harness.Game.State.IsDocked);
        Assert.False(runs.FastFlight.Harness.Game.State.IsDocked);
    }

    [Fact]
    public void TheShipHasTravelledTheSameDistance()
    {
        // Not to the bit. The slow run takes one long step where the fast one
        // takes four and a bit short ones, and the rotation the port uses is
        // a first-order approximation, so the two diverge slightly. What
        // matters is that they diverge by a fraction of a percent rather than
        // by the four-times factor the conversion exists to remove.
        float slowDistance = DistanceToPlanet(runs.SlowFlight);
        float fastDistance = DistanceToPlanet(runs.FastFlight);

        Assert.True(slowDistance > 0, "the slow run never found a planet to measure against");
        Assert.Equal(slowDistance, fastDistance, slowDistance * 0.02f);
    }

    // The strongest claim of the lot, and only true since the starfield
    // stopped drawing from the game's random stream. Encounters are rolled on
    // the housekeeping clock, so the same seconds roll the same dice - but
    // only if nothing else has been drawing from the stream once per update in
    // between. The starfield was, and this is what says it no longer is.
    [Fact]
    public void BothRatesMeetTheSameShips()
        => Assert.Equal(ShipTypes(runs.SlowFlight), ShipTypes(runs.FastFlight));

    [Fact]
    public void TheThrottleRampsAtTheSameSpeed()
    {
        // Sampled part way up the ramp, on purpose. The tests above compare
        // the end of a long flight, where both runs have been at maximum
        // speed for most of it and a throttle that winds up four times too
        // fast is invisible. This one reads the speed as the run goes past
        // the middle of the ramp, which is where that bug lived.
        float slowSpeed = runs.SlowFlight.Speeds[0];

        Assert.InRange(slowSpeed, 13f, runs.SlowFlight.Harness.Resolve<PlayerShip>().MaxSpeed - 1);
        Assert.Equal(slowSpeed, runs.FastFlight.Speeds[0], 1.5f);
    }

    [Fact]
    public void TheDockingComputerWindsTheThrottleDownAtTheSameSpeed()
    {
        // The docking computer's own throttle, which the player's key never
        // touches: the autopilot nudges the speed a step an update, so it
        // needed the same scaling as everything else. It is engaged at full
        // speed pointing away from the station, so the first thing it does is
        // wind the throttle down - and this reads part way down, for the same
        // reason as the test above. The ramp saturates at both ends, and a
        // sample taken at rest cannot tell a correct rate from a fourfold
        // one; that is what sank the first attempt at this test.
        float slowSpeed = runs.SlowDocking.Speeds[0];

        Assert.InRange(slowSpeed, 5f, 21f);
        Assert.Equal(slowSpeed, runs.FastDocking.Speeds[0], 1.5f);
    }

    [Fact]
    public void TheDockingComputerWindsTheThrottleUpAtTheSameSpeed()
    {
        // The other half of the same method. Once the computer has turned the
        // ship around it accelerates towards the station, and this reads the
        // speed while it is still doing so: the wind-down test above passes
        // with the accelerating branch left unscaled, so the approach has to
        // be caught mid-ramp to cover it.
        float slowSpeed = runs.SlowDocking.Speeds[1];

        Assert.InRange(slowSpeed, 2f, 21f);
        Assert.Equal(slowSpeed, runs.FastDocking.Speeds[1], 1.5f);
    }

    private static string ShipTypes(FrameRateRun run)
        => string.Join(
            ", ",
            run.Harness.Resolve<Universe>().GetAllObjects().Select(o => o.Type.ToString()).Order());

    private static float DistanceToPlanet(FrameRateRun run)
    {
        foreach (IObject obj in run.Harness.Resolve<Universe>().GetAllObjects())
        {
            if (obj.Type == ShipType.Planet)
            {
                return obj.Location.Length();
            }
        }

        return 0;
    }
}
