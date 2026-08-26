// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.Input;

namespace EliteSharpLib.Tests.GoldenTrace;

// One recorded run: a fixed seed, a fixed key script and a fixed length, so
// the same scenario always produces the same trace.
//
// FrameTicks names the ticks whose composed frame is also checked against a
// committed signature. Deliberately a handful rather than every tick: the
// traces already cover what the game is doing, and the frames are there to
// cover what the traces cannot see - the order things are drawn in.
internal sealed record TraceScenario(
    string Name,
    int RandomSeed,
    int Ticks,
    IReadOnlyList<KeyScriptEvent> Script,
    IReadOnlyList<int> FrameTicks);
