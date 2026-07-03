// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Tracks;

// A y.coordinate.offsets template in original Amiga encoding.
internal sealed class AmigaPieceY(bool words, byte[] data)
{
    // True when the y coords are stored as 15-bit big-endian words
    // (e.g. steeper sections on the roller coaster or the high jump).
    internal bool Words { get; } = words;

    internal byte[] Data { get; } = data;
}
