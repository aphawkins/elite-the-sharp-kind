// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Views;

namespace EliteSharpLib.Tests.GoldenTrace;

// Everything one tick of the game is, as far as the rate rework can disturb
// it. Deliberately not a rendered frame: the seven frame-rate items change
// when things happen, not what they look like, and a pixel comparison would
// fail for reasons that have nothing to do with them.
//
// Note there is no player position. Elite holds the player at the origin and
// moves the universe past it, so the player's motion shows up in every
// object's Location rather than in a coordinate of its own.
internal sealed record TraceSample(
    int Tick,
    Screen Screen,
    bool IsDocked,
    bool IsGameOver,
    int MCount,
    int MessageCount,
    float LaserTemp,
    float Roll,
    float Climb,
    float Speed,
    float Energy,
    float ShieldFront,
    float ShieldRear,
    float Fuel,
    float CabinTemperature,
    float Altitude,
    IReadOnlyList<TraceObject> Objects);
