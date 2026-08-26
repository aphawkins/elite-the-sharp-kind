// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.Input;

namespace EliteSharpLib.Tests.GoldenTrace;

// One recorded run: a fixed seed, a fixed key script and a fixed length, so
// the same scenario always produces the same trace.
internal sealed record TraceScenario(
    string Name,
    int RandomSeed,
    int Ticks,
    IReadOnlyList<KeyScriptEvent> Script);
