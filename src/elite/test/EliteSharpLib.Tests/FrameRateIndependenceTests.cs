// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using SharpKind.Input;

namespace EliteSharpLib.Tests;

/// <summary>
/// The same stretch of game played at two different update rates.
/// </summary>
/// <remarks>
/// This is what the whole frame-rate rework was for, and the only test that
/// puts the pieces together: the housekeeping clock, the motion, the
/// animations and the AI pacing all converted separately, each proved
/// unchanged at the game's own 13.5Hz. Here the game is run at 60Hz for the
/// same number of seconds and asked to be in the same place.
/// </remarks>
public class FrameRateIndependenceTests
{
    private const float FastRate = 60f;
    private const float Ratio = FastRate / GameClock.StepsPerSecond;

    // Long enough to launch, fly, and take a few hundred housekeeping steps.
    private const int SlowUpdates = 300;

    // The update the accelerate key goes down on, at the game's own rate.
    private const int LaunchUpdates = 40;

    // The update the docking computer is engaged on: far enough in that the
    // ship is at full speed and well clear of the station.
    private const int EngageUpdates = 80;

    // The updates the two halves of the computer's throttle ramp are sampled
    // at - one while it is still slowing the ship down, one after it has
    // turned around and while it is still speeding it back up.
    private const int WindDownUpdates = 108;
    private const int WindUpUpdates = 175;

    [Fact]
    public void TheHousekeepingTakesTheSameStepsInTheSameTime()
    {
        // Within one step. The count is driven by elapsed time and the clock
        // keeps its unspent remainder, so 300 updates at 13.5Hz and 1333 at
        // 60Hz are the same 22 seconds and buy the same 276-odd steps; the
        // one that can differ is whichever the truncation of 1333 leaves
        // half-finished.
        using HeadlessGameHarness slow = Launch(GameClock.StepsPerSecond, SlowUpdates);
        using HeadlessGameHarness fast = Launch(FastRate, (int)(SlowUpdates * Ratio));

        Assert.InRange(fast.Game.State.MCount, slow.Game.State.MCount - 1, slow.Game.State.MCount + 1);
    }

    [Fact]
    public void TheGameEndsUpOnTheSameScreenHavingLaunched()
    {
        using HeadlessGameHarness slow = Launch(GameClock.StepsPerSecond, SlowUpdates);
        using HeadlessGameHarness fast = Launch(FastRate, (int)(SlowUpdates * Ratio));

        Assert.Equal(Screen.FrontView, slow.Game.State.CurrentScreen);
        Assert.Equal(Screen.FrontView, fast.Game.State.CurrentScreen);
        Assert.False(slow.Game.State.IsDocked);
        Assert.False(fast.Game.State.IsDocked);
    }

    [Fact]
    public void TheShipHasTravelledTheSameDistance()
    {
        // Not to the bit. The slow run takes one long step where the fast one
        // takes four and a bit short ones, and the rotation the port uses is
        // a first-order approximation, so the two diverge slightly. What
        // matters is that they diverge by a fraction of a percent rather than
        // by the four-times factor the conversion exists to remove.
        using HeadlessGameHarness slow = Launch(GameClock.StepsPerSecond, SlowUpdates);
        using HeadlessGameHarness fast = Launch(FastRate, (int)(SlowUpdates * Ratio));

        float slowDistance = DistanceToPlanet(slow);
        float fastDistance = DistanceToPlanet(fast);

        Assert.True(slowDistance > 0, "the slow run never found a planet to measure against");
        Assert.Equal(slowDistance, fastDistance, slowDistance * 0.02f);
    }

    [Fact]
    public void BothRatesMeetTheSameShips()
    {
        // The strongest claim of the lot, and only true since the starfield
        // stopped drawing from the game's random stream. Encounters are
        // rolled on the housekeeping clock, so the same seconds roll the same
        // dice - but only if nothing else has been drawing from the stream
        // once per update in between. The starfield was, and this is what
        // says it no longer is.
        using HeadlessGameHarness slow = Launch(GameClock.StepsPerSecond, SlowUpdates);
        using HeadlessGameHarness fast = Launch(FastRate, (int)(SlowUpdates * Ratio));

        Assert.Equal(ShipTypes(slow), ShipTypes(fast));
    }

    [Fact]
    public void TheThrottleRampsAtTheSameSpeed()
    {
        // Sampled part way up the ramp, on purpose. The tests above compare
        // the end of a long flight, where both runs have been at maximum
        // speed for most of it and a throttle that winds up four times too
        // fast is invisible. This one stops while the throttle is still
        // moving, which is where that bug lived.
        using HeadlessGameHarness slow = Launch(GameClock.StepsPerSecond, LaunchUpdates + 13);
        using HeadlessGameHarness fast = Launch(FastRate, (int)((LaunchUpdates + 13) * Ratio));

        float slowSpeed = slow.Resolve<PlayerShip>().Speed;
        Assert.InRange(slowSpeed, 13f, slow.Resolve<PlayerShip>().MaxSpeed - 1);
        Assert.Equal(slowSpeed, fast.Resolve<PlayerShip>().Speed, 1.5f);
    }

    [Fact]
    public void TheDockingComputerWindsTheThrottleDownAtTheSameSpeed()
    {
        // The docking computer's own throttle, which the player's key never
        // touches: the autopilot nudges the speed a step an update, so it
        // needed the same scaling as everything else. It is engaged at full
        // speed pointing away from the station, so the first thing it does is
        // wind the throttle down - and this samples part way down, for the
        // same reason as the test above. The ramp saturates at both ends, and
        // a sample taken at rest cannot tell a correct rate from a fourfold
        // one; that is what sank the first attempt at this test.
        using HeadlessGameHarness slow = Dock(GameClock.StepsPerSecond, WindDownUpdates);
        using HeadlessGameHarness fast = Dock(FastRate, (int)(WindDownUpdates * Ratio));

        float slowSpeed = slow.Resolve<PlayerShip>().Speed;
        Assert.InRange(slowSpeed, 5f, 21f);
        Assert.Equal(slowSpeed, fast.Resolve<PlayerShip>().Speed, 1.5f);
    }

    [Fact]
    public void TheDockingComputerWindsTheThrottleUpAtTheSameSpeed()
    {
        // The other half of the same method. Once the computer has turned the
        // ship around it accelerates towards the station, and the run is
        // stopped while it is still doing so: the wind-down test above passes
        // with the accelerating branch left unscaled, so the approach has to
        // be caught mid-ramp to cover it.
        using HeadlessGameHarness slow = Dock(GameClock.StepsPerSecond, WindUpUpdates);
        using HeadlessGameHarness fast = Dock(FastRate, (int)(WindUpUpdates * Ratio));

        float slowSpeed = slow.Resolve<PlayerShip>().Speed;
        Assert.InRange(slowSpeed, 2f, 21f);
        Assert.Equal(slowSpeed, fast.Resolve<PlayerShip>().Speed, 1.5f);
    }

    private static string ShipTypes(HeadlessGameHarness harness)
        => string.Join(
            ", ",
            harness.Resolve<Universe>().GetAllObjects().Select(o => o.Type.ToString()).Order());

    // Runs the game to <paramref name="updates"/> at the given rate, with the
    // key script stretched so each press lands at the same moment in time
    // rather than on the same update number.
    private static HeadlessGameHarness Launch(float updatesPerSecond, int updates)
    {
        float scale = updatesPerSecond / GameClock.StepsPerSecond;
        KeyScriptEvent[] script =
        [
            new((int)(1 * scale), ConsoleKey.N, KeyScriptAction.Tap),
            new((int)(2 * scale), ConsoleKey.Spacebar, KeyScriptAction.Tap),
            new((int)(4 * scale), ConsoleKey.F1, KeyScriptAction.Tap),
            new((int)(40 * scale), ConsoleKey.Spacebar, KeyScriptAction.Hold),
            new((int)(90 * scale), ConsoleKey.Spacebar, KeyScriptAction.Release),
        ];

        HeadlessGameHarness harness = new(randomSeed: 4242, updatesPerSecond: updatesPerSecond);
        harness.Run(updates, script);
        return harness;
    }

    // Launches as above, then hands the ship a docking computer it did not
    // start with and engages it. The equipment is fitted mid-run because a
    // new commander has none, and 'N' at the start of the script makes one.
    private static HeadlessGameHarness Dock(float updatesPerSecond, int updates)
    {
        float scale = updatesPerSecond / GameClock.StepsPerSecond;
        int engage = (int)(EngageUpdates * scale);
        KeyScriptEvent[] script =
        [
            new((int)(1 * scale), ConsoleKey.N, KeyScriptAction.Tap),
            new((int)(2 * scale), ConsoleKey.Spacebar, KeyScriptAction.Tap),
            new((int)(4 * scale), ConsoleKey.F1, KeyScriptAction.Tap),
            new((int)(40 * scale), ConsoleKey.Spacebar, KeyScriptAction.Hold),
            new((int)(70 * scale), ConsoleKey.Spacebar, KeyScriptAction.Release),
            new(engage, ConsoleKey.C, KeyScriptAction.Tap),
        ];

        HeadlessGameHarness harness = new(randomSeed: 4242, updatesPerSecond: updatesPerSecond);
        harness.Run(engage, script);
        harness.Resolve<PlayerShip>().HasDockingComputer = true;
        harness.Run(updates - engage, script);
        return harness;
    }

    private static float DistanceToPlanet(HeadlessGameHarness harness)
    {
        foreach (IObject obj in harness.Resolve<Universe>().GetAllObjects())
        {
            if (obj.Type == ShipType.Planet)
            {
                return obj.Location.Length();
            }
        }

        return 0;
    }
}
