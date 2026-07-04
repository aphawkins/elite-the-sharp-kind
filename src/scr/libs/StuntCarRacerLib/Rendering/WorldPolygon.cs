// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Tracks;

namespace StuntCarRacerLib.Rendering;

// A flat-shaded polygon in world track units, drawn depth-sorted with the track.
public sealed class WorldPolygon(Coord3D[] points, uint colour)
{
    private readonly Coord3D[] _points = points;

    public uint Colour { get; } = colour;

    internal ReadOnlySpan<Coord3D> Points => _points;
}
