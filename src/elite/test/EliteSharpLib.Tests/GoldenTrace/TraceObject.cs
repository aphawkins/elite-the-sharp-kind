// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Tests.GoldenTrace;

// One universe slot as the trace records it. The player is always at the
// origin in Elite, so a slot's Location is already relative to the ship and
// needs no camera applied to be meaningful.
//
// ExpDelta is the explosion cloud's age, and it is here because it is the
// one piece of game state the renderer currently owns: EliteDraw seeds it
// and advances it by four every drawn frame. Separating simulate from
// compose moves that advance, and without ExpDelta in the trace the move
// would only show up indirectly, as the tick a wreck is finally removed.
internal readonly record struct TraceObject(
    int Slot,
    string Type,
    float X,
    float Y,
    float Z,
    float RotX,
    float RotZ,
    string Flags,
    int ExpDelta);
