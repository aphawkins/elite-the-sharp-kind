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
