// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Tests;

public class NearPlaneClipTests
{
    private const float NearPlane = 0.5f;

    [Fact]
    public void PolygonBehindCameraIsFullyClipped()
    {
        Vector3[] polygon =
        [
            new(-100, 0, -1000),
            new(100, 0, -1000),
            new(100, 0, -500),
            new(-100, 0, -500),
        ];

        Span<Vector3> output = stackalloc Vector3[5];
        int count = NearPlaneClip.Clip(polygon, NearPlane, output);

        Assert.Equal(0, count);
    }

    [Fact]
    public void PolygonInFrontIsUnchanged()
    {
        Vector3[] polygon =
        [
            new(-100, 0, 1000),
            new(100, 0, 1000),
            new(100, 0, 500),
            new(-100, 0, 500),
        ];

        Span<Vector3> output = stackalloc Vector3[5];
        int count = NearPlaneClip.Clip(polygon, NearPlane, output);

        Assert.Equal(4, count);
        Assert.Equal(polygon[0], output[0]);
        Assert.Equal(polygon[3], output[3]);
    }

    [Fact]
    public void PolygonCrossingNearPlaneIsClippedToBoundary()
    {
        Vector3[] polygon =
        [
            new(-100, 0, 1000),
            new(100, 0, 1000),
            new(100, 0, -1000),
            new(-100, 0, -1000),
        ];

        Span<Vector3> output = stackalloc Vector3[5];
        int count = NearPlaneClip.Clip(polygon, NearPlane, output);

        Assert.Equal(4, count);
        for (int i = 0; i < count; i++)
        {
            Assert.True(output[i].Z >= NearPlane);
        }
    }

    [Fact]
    public void TextureCoordinatesAreInterpolatedThroughTheClip()
    {
        Vector3[] polygon =
        [
            new(0, 0, 1),
            new(10, 0, 1),
            new(10, 0, -1),
            new(0, 0, -1),
        ];
        Vector2[] uv = [new(0, 0), new(1, 0), new(1, 1), new(0, 1)];

        Span<Vector3> output = stackalloc Vector3[5];
        Span<Vector2> outputUv = stackalloc Vector2[5];
        int count = NearPlaneClip.Clip(polygon, uv, NearPlane, output, outputUv);

        Assert.Equal(4, count);
        for (int i = 0; i < count; i++)
        {
            Assert.InRange(outputUv[i].X, 0, 1);
            Assert.InRange(outputUv[i].Y, 0, 1);
        }

        // The clipped points sit a quarter of the way along the two edges
        // that cross the plane, so their v follows.
        Assert.Equal(0.25f, outputUv[2].Y, 3);
        Assert.Equal(0.25f, outputUv[3].Y, 3);
    }

    // A Gouraud face is shaded at its own corners, so a corner the clipper
    // invents needs the colour the face had where the plane cut it.
    [Fact]
    public void ColoursAreInterpolatedThroughTheClip()
    {
        Vector3[] polygon =
        [
            new(0, 0, 1),
            new(10, 0, 1),
            new(10, 0, -1),
            new(0, 0, -1),
        ];

        // Black at the far edge, white at the near one.
        FastColor[] colours =
        [
            new(255, 0, 0, 0),
            new(255, 0, 0, 0),
            new(255, 255, 255, 255),
            new(255, 255, 255, 255),
        ];

        Span<Vector3> output = stackalloc Vector3[5];
        Span<FastColor> outputColours = stackalloc FastColor[5];
        int count = NearPlaneClip.Clip(polygon, colours, NearPlane, output, outputColours);

        Assert.Equal(4, count);

        // The two points that survive keep their own colour.
        Assert.Equal(colours[0], outputColours[0]);
        Assert.Equal(colours[1], outputColours[1]);

        // The clipped points sit a quarter of the way towards the white
        // corners, so they come out a quarter of the way to white.
        Assert.Equal(new FastColor(255, 64, 64, 64), outputColours[2]);
        Assert.Equal(new FastColor(255, 64, 64, 64), outputColours[3]);
    }

    [Fact]
    public void ADifferentNearPlaneMovesTheBoundary()
    {
        Vector3[] polygon = [new(0, 0, 10), new(10, 0, 10), new(5, 0, -10)];

        Span<Vector3> output = stackalloc Vector3[4];
        int count = NearPlaneClip.Clip(polygon, 1f, output);

        Assert.Equal(4, count);
        Assert.Equal(1f, output[2].Z);
        Assert.Equal(1f, output[3].Z);
    }
}
