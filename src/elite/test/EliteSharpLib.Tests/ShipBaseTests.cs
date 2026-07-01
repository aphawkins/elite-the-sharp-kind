// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using Useful.Assets.Models;
using Useful.Maths;

namespace EliteSharpLib.Tests;

public class ShipBaseTests
{
    private const float Tolerance = 0.01f;

    [Fact]
    public void DrawTransformsPointsUsingRotationMatrixAfterAxisSwap()
    {
        // Arrange: a non-orthonormal Rotmat with distinct off-diagonal values, so that the
        // pre-swap and post-swap matrices project points to different screen positions.
        Vector4[] rotmat =
        [
            new(1.0f, 0.2f, 0.3f, 0),
            new(0.4f, 1.0f, 0.5f, 0),
            new(0.6f, 0.7f, 1.0f, 0),
            new(0, 0, 0, 0),
        ];

        Vector4 location = new(100, 200, 5000, 0);
        Vector4 pointA = new(10, 20, 30, 0);
        Vector4 pointB = new(5, -15, 25, 0);

        Point modelPointA = new() { Coords = pointA, Distance = 0, FaceNormals = [] };
        Point modelPointB = new() { Coords = pointB, Distance = 0, FaceNormals = [] };

        // A 2-point "face" always passes the visibility/winding check in DrawModelFaces
        // (point0 and point2 resolve to the same point), keeping this test independent of it.
        Face face = new() { Color = 0, Normal = Vector4.Zero, Points = [modelPointA, modelPointB] };

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw)
        {
            Rotmat = rotmat,
            Location = location,
            Model = new()
            {
                FaceNormals = [],
                Faces = [face],
                Lines = [],
                Points = [modelPointA, modelPointB],
            },
        };

        // Act
        ship.Draw();

        // Assert
        Vector2 expectedA = ProjectWithSwappedMatrix(pointA, rotmat, location, draw);
        Vector2 expectedB = ProjectWithSwappedMatrix(pointB, rotmat, location, draw);
        Vector2 wrongA = ProjectWithUnswappedMatrix(pointA, rotmat, location, draw);

        (Vector2[] points, uint _, float _) = Assert.Single(draw.DrawnPolygons);
        Assert.Equal(2, points.Length);

        AssertVector2AlmostEqual(expectedA, points[0]);
        AssertVector2AlmostEqual(expectedB, points[1]);

        // Regression check: Draw() must not use the pre-swap matrix (the bug fixed in ShipBase.cs).
        Assert.False(
            Math.Abs(points[0].X - wrongA.X) < Tolerance && Math.Abs(points[0].Y - wrongA.Y) < Tolerance,
            "Draw() should use the axis-swapped rotation matrix, not the pre-swap one.");
    }

    private static Vector2 ProjectWithSwappedMatrix(Vector4 point, Vector4[] rotmat, Vector4 location, FakeEliteDraw draw)
    {
        Vector4[] transMat = [.. rotmat];

        (transMat[1].X, transMat[0].Y) = (transMat[0].Y, transMat[1].X);
        (transMat[2].X, transMat[0].Z) = (transMat[0].Z, transMat[2].X);
        (transMat[2].Y, transMat[1].Z) = (transMat[1].Z, transMat[2].Y);

        return Project(point, transMat, location, draw);
    }

    private static Vector2 ProjectWithUnswappedMatrix(Vector4 point, Vector4[] rotmat, Vector4 location, FakeEliteDraw draw)
        => Project(point, rotmat, location, draw);

    private static Vector2 Project(Vector4 point, Vector4[] transMat, Vector4 location, FakeEliteDraw draw)
    {
        Vector4 vec = Vector4.Transform(point, transMat.ToMatrix4x4());
        vec += location;

        if (vec.Z <= 0)
        {
            vec.Z = 1;
        }

        float x = ((vec.X * 256 / vec.Z) + (draw.Centre.X / 2)) * draw.Graphics.Scale;
        float y = ((-vec.Y * 256 / vec.Z) + (draw.Centre.Y / 2)) * draw.Graphics.Scale;
        return new(x, y);
    }

    private static void AssertVector2AlmostEqual(Vector2 expected, Vector2 actual)
    {
        Assert.InRange(actual.X, expected.X - Tolerance, expected.X + Tolerance);
        Assert.InRange(actual.Y, expected.Y - Tolerance, expected.Y + Tolerance);
    }
}
