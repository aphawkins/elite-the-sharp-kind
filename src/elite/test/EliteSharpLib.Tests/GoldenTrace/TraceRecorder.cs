// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Save;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Tests.GoldenTrace;

// Drives a scenario through the headless harness and snapshots the game
// after every tick.
//
// Only Update() is called, never Draw(): Elite composes its whole frame
// inside Update today and Draw only presents it, so the simulation is
// complete without presenting. That also keeps the recording off
// EliteMain.Draw's Stopwatch, which is the one piece of wall-clock time in
// the loop and would make a trace unreproducible.
internal static class TraceRecorder
{
    internal static IReadOnlyList<TraceSample> Record(TraceScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        // A trace must record the commander the game ships with, not the one
        // the developer's machine happens to ask for. ELITE_DEBUG_COMMANDER
        // is read in SaveFile's constructor and swaps Jameson for a fully
        // equipped Max - including a stronger front laser, which changes the
        // firing cadence and so the whole laser-fire trace. Cleared here for
        // the same reason SaveFileTests clears it, and clearing rather than
        // setting keeps the two agreeing whichever order they run in.
        using EnvironmentVariableScope commander =
            EnvironmentVariableScope.Set(SaveFile.DebugCommanderEnvVar, null);

        using HeadlessGameHarness harness = new(randomSeed: scenario.RandomSeed);
        PlayerShip ship = harness.Resolve<PlayerShip>();
        Universe universe = harness.Resolve<Universe>();
        GameState state = harness.Game.State;

        List<TraceSample> samples = new(scenario.Ticks);
        for (int tick = 0; tick < scenario.Ticks; tick++)
        {
            harness.Step(scenario.Script);
            samples.Add(Capture(tick, state, ship, universe));
        }

        return samples;
    }

    private static TraceSample Capture(int tick, GameState state, PlayerShip ship, Universe universe)
    {
        List<TraceObject> objects = [];
        int slot = -1;
        foreach (IObject obj in universe.GetAllObjects())
        {
            slot++;

            // An empty slot is still a slot: Combat.Tactics phases a ship's
            // AI by its slot index, so the trace has to keep the numbering
            // rather than compact the list.
            if (obj.Id == ObjectIds.None)
            {
                continue;
            }

            objects.Add(new(
                slot,
                obj.Id,
                obj.Location.X,
                obj.Location.Y,
                obj.Location.Z,
                obj.RotX,
                obj.RotZ,
                obj.Flags.ToString(),

                // Planets and suns are IObject but not IShip, and only a ship
                // ever carries an explosion.
                obj is IShip wreck ? wreck.ExpDelta : 0));
        }

        return new(
            tick,
            state.CurrentScreen,
            state.IsDocked,
            state.IsGameOver,
            state.MCount,
            state.MessageCount,
            state.LaserTemp,
            ship.Roll,
            ship.Pitch,
            ship.Speed,
            ship.Energy,
            ship.ShieldFront,
            ship.ShieldRear,
            ship.Fuel,
            ship.CabinTemperature,
            ship.Altitude,
            objects);
    }
}
