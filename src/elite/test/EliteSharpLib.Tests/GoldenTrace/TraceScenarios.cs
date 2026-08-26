// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.Input;

namespace EliteSharpLib.Tests.GoldenTrace;

// The scenarios the baselines cover. Between them they exercise every rate
// and counter the seven frame-rate items in backlog-roadmap.md touch; the
// comment on each says which, so a scenario is not dropped later without
// noticing what it was holding down.
//
// Tick 0 runs InitialiseGame(), which sets the view to IntroOne and clears
// any pressed keys on the way in - so a script's first key can only land on
// tick 1.
internal static class TraceScenarios
{
    // Elite's own choice of seed matters only in that it never changes.
    private const int Seed = 20260826;

    // The title screen and the ship parade. Covers Intro1Controller's
    // Z -= 100 per tick, Intro2Controller's _showTime >= 140 turnaround and
    // its per-tick Z step, and - because the parade ship is spun by
    // AddNewShip's rotx/rotz - Space.SpinUniverseObject and RotateXFirst.
    internal static TraceScenario IntroParade { get; } = new(
        "intro-parade",
        Seed,
        260,
        [
            new(1, ConsoleKey.N, KeyScriptAction.Tap),
        ]);

    // Docked, then launched, then flown. Covers BreakPattern's 20 rings (the
    // launch animation), Space.LaunchPlayer, the station and planet moving
    // past under MoveUniverseObject/ApplyShipVelocity, the front starfield's
    // per-tick delta, and the player's roll/climb ramp - which
    // PilotController steps twice a tick.
    internal static TraceScenario LaunchAndFly { get; } = new(
        "launch-and-fly",
        Seed,
        200,
        [
            new(1, ConsoleKey.N, KeyScriptAction.Tap),
            new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),

            // F1 while docked is Undocking, which is LaunchView.
            new(4, ConsoleKey.F1, KeyScriptAction.Tap),

            // Accelerate for a while, then roll and climb into it.
            new(40, ConsoleKey.Spacebar, KeyScriptAction.Hold),
            new(90, ConsoleKey.Spacebar, KeyScriptAction.Release),
            new(90, ConsoleKey.OemPeriod, KeyScriptAction.Hold),
            new(130, ConsoleKey.OemPeriod, KeyScriptAction.Release),
            new(130, ConsoleKey.S, KeyScriptAction.Hold),
            new(170, ConsoleKey.S, KeyScriptAction.Release),
        ]);

    // The same launch, then left alone long enough for MCount to wrap. That
    // is what this one is for: MCount counts 255 down to 0, so only a run
    // this long reaches the shield regen (& 7), the energy-low and altitude
    // check (& 31 == 10), the cabin temperature (& 31 == 20) and the
    // random encounter (== 0) - and, once an encounter arrives, Combat's
    // one-in-eight tactics pacing.
    internal static TraceScenario LongFlight { get; } = new(
        "long-flight",
        Seed,
        600,
        [
            new(1, ConsoleKey.N, KeyScriptAction.Tap),
            new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),
            new(4, ConsoleKey.F1, KeyScriptAction.Tap),
            new(40, ConsoleKey.Spacebar, KeyScriptAction.Hold),
            new(120, ConsoleKey.Spacebar, KeyScriptAction.Release),
        ]);

    // Launched, then the trigger held down. Covers what the flying scenarios
    // never touch: Combat.FireLaser, the laser temperature climbing and
    // CoolLaser's per-tick -2, and PilotController's own _drawLaserFrames
    // countdown.
    internal static TraceScenario LaserFire { get; } = new(
        "laser-fire",
        Seed,
        160,
        [
            new(1, ConsoleKey.N, KeyScriptAction.Tap),
            new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),
            new(4, ConsoleKey.F1, KeyScriptAction.Tap),

            // Held well past the point the laser overheats and cuts out, so
            // the cooling ramp is in the trace as well as the heating one.
            new(30, ConsoleKey.A, KeyScriptAction.Hold),
            new(110, ConsoleKey.A, KeyScriptAction.Release),
        ]);

    internal static IReadOnlyList<TraceScenario> All { get; } =
        [IntroParade, LaunchAndFly, LongFlight, LaserFire];
}
