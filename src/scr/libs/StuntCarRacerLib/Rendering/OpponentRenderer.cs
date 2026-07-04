// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Tracks;
using Useful;

namespace StuntCarRacerLib.Rendering;

// Draws the opponent as a simple flat-shaded box on its wheel positions,
// plus its road shadow (the original draws a proper car mesh from Car.cpp,
// which is not ported yet). Uses the original's car colours 2 palette.
public sealed class OpponentRenderer
{
    // A low box; the original visible car height is 162 track units at the cab.
    private const int BodyHeight = 60;

    private const int ShadowColour = Track.ScrBaseColour + 5;

    private const int BodyColour = 21; // car colours 2 mid blue

    private const int TopColour = 22; // car colours 2 light blue

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

        Coord3D rl = _opponent.VisualRearLeft;
        Coord3D rr = _opponent.VisualRearRight;
        Coord3D fl = _opponent.VisualFrontLeft;
        Coord3D fr = _opponent.VisualFrontRight;

        Coord3D topRearLeft = rl with { Y = rl.Y + BodyHeight };
        Coord3D topRearRight = rr with { Y = rr.Y + BodyHeight };
        Coord3D topFrontLeft = fl with { Y = fl.Y + BodyHeight };
        Coord3D topFrontRight = fr with { Y = fr.Y + BodyHeight };

        uint body = ScrPalette.Colour(BodyColour);
        uint top = ScrPalette.Colour(TopColour);

        // four sides and the top (no bottom; it is never visible)
        polygons.Add(new([rl, rr, topRearRight, topRearLeft], body)); // rear
        polygons.Add(new([fl, fr, topFrontRight, topFrontLeft], body)); // front
        polygons.Add(new([rl, fl, topFrontLeft, topRearLeft], body)); // left
        polygons.Add(new([rr, fr, topFrontRight, topRearRight], body)); // right
        polygons.Add(new([topRearLeft, topRearRight, topFrontRight, topFrontLeft], top)); // top
    }
}
