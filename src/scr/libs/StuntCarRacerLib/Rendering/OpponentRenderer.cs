// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Tracks;
using Useful;

namespace StuntCarRacerLib.Rendering;

// Draws the opponent using the Car.cpp mesh oriented on its wheel
// positions, plus its road shadow.
public sealed class OpponentRenderer
{
    private const int ShadowColour = Track.ScrBaseColour + 5;

    private readonly OpponentPhysics _opponent;

    public OpponentRenderer(OpponentPhysics opponent)
    {
        Guard.ArgumentNull(opponent);
        _opponent = opponent;
    }

    // Appends the opponent's polygons (world track units) for depth-sorted
    // drawing with the track.
    public void AppendWorldPolygons(ICollection<WorldPolygon> polygons)
    {
        Guard.ArgumentNull(polygons);

        if (_opponent.OpponentId < 0)
        {
            return;
        }

        // shadow quad, slightly above the road
        if (_opponent.ShadowVisible)
        {
            polygons.Add(new(
                [_opponent.ShadowRearLeft, _opponent.ShadowRearRight, _opponent.ShadowFrontRight, _opponent.ShadowFrontLeft],
                ScrPalette.Colour(ShadowColour)));
        }

        CarMesh.Append(
            polygons,
            _opponent.VisualRearLeft,
            _opponent.VisualRearRight,
            _opponent.VisualFrontLeft,
            _opponent.VisualFrontRight);
    }
}
