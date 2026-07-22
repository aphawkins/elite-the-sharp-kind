// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Rendering;
using StuntCarRacerSharpLib.Tracks;
using Useful;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Rendering;

public class CarMeshTests
{
    // A level, square wheel-corner frame: right = +X, forward = +Z, up = +Y.
    private static readonly Coord3D s_rearLeft = new(-81, 0, -128);
    private static readonly Coord3D s_rearRight = new(81, 0, -128);
    private static readonly Coord3D s_frontLeft = new(-81, 0, 128);
    private static readonly Coord3D s_frontRight = new(81, 0, 128);

    // Regression lock for the car.obj conversion: same quad count and
    // colours as the original hardcoded CreateCarInVB-derived arrays.
    [Fact]
    public void AppendProducesTenQuadsWithTheOriginalColours()
    {
        ScrPalette palette = new();
        CarMesh carMesh = new(palette);
        List<WorldPolygon> polygons = [];

        carMesh.Append(polygons, s_rearLeft, s_rearRight, s_frontLeft, s_frontRight);

        Assert.Equal(10, polygons.Count);
        Assert.All(polygons, p => Assert.Equal(4, p.Points.Length));

        FastColor wheelColour = palette.Colour(Track.ScrBaseColour);
        FastColor sideColour = palette.Colour(Track.ScrBaseColour + 12);
        FastColor endColour = palette.Colour(Track.ScrBaseColour + 10);
        FastColor topColour = palette.Colour(Track.ScrBaseColour + 15);
        FastColor bottomColour = palette.Colour(Track.ScrBaseColour + 9);

        Assert.Equal(4, polygons.Count(p => p.Colour == wheelColour));
        Assert.Equal(2, polygons.Count(p => p.Colour == sideColour));
        Assert.Equal(2, polygons.Count(p => p.Colour == endColour));
        Assert.Single(polygons, p => p.Colour == topColour);
        Assert.Single(polygons, p => p.Colour == bottomColour);
    }

    // The mesh bottom must sit exactly at wheel-corner level, whatever the
    // orientation frame - here a level frame, so the wheel quads' lowest
    // points should land at the corners' Y (0).
    [Fact]
    public void MeshBottomSitsAtCornerLevelOnALevelFrame()
    {
        CarMesh carMesh = new(new ScrPalette());
        List<WorldPolygon> polygons = [];

        carMesh.Append(polygons, s_rearLeft, s_rearRight, s_frontLeft, s_frontRight);

        int lowestY = polygons.SelectMany(p => p.Points.ToArray()).Min(c => c.Y);

        Assert.Equal(0, lowestY);
    }

    [Fact]
    public void AppendThrowsOnNullPolygons()
    {
        CarMesh carMesh = new(new ScrPalette());

        Assert.Throws<ArgumentNullException>(
            () => carMesh.Append(null!, s_rearLeft, s_rearRight, s_frontLeft, s_frontRight));
    }
}
