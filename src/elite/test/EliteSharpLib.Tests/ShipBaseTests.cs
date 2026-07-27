// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using Useful.Assets.Models;
using Useful.Fakes;

namespace EliteSharpLib.Tests;

public class ShipBaseTests
{
    private const float Tolerance = 0.01f;

    [Fact]
    public void DrawTransformsModelPointsUsingRotmatBasisVectors()
    {
        // Arrange: a non-orthonormal Rotmat with distinct values, so every basis vector's
        // contribution to the result is independently observable.
        Matrix4x4 rotmat = new(
            1.0f,
            0.2f,
            0.3f,
            0,
            0.4f,
            1.0f,
            0.5f,
            0,
            0.6f,
            0.7f,
            1.0f,
            0,
            0,
            0,
            0,
            0);

        Vector4 location = new(100, 200, 5000, 0);
        Vector4 pointA = new(10, 20, 30, 0);
        Vector4 pointB = new(5, -15, 25, 0);

        Point modelPointA = new() { Coords = pointA, FaceNormals = [] };
        Point modelPointB = new() { Coords = pointB, FaceNormals = [] };

        // A 2-point "face" always passes the visibility/winding check in DrawModelFaces
        // (point0 and point2 resolve to the same point), keeping this test independent of it.
        Face face = new() { Color = 0, Points = [modelPointA, modelPointB], PointIndices = [0, 1] };

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new Random(0)))
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

        // Assert: Rotmat[0..2] are the object's basis vectors, so a model-local point should be
        // transformed into world space as p.X*Rotmat[0] + p.Y*Rotmat[1] + p.Z*Rotmat[2] + Location,
        // regardless of how Rotmat happens to be stored internally as a Matrix4x4.
        Vector2 expectedA = ProjectUsingRotmatBasis(pointA, rotmat, location, draw);
        Vector2 expectedB = ProjectUsingRotmatBasis(pointB, rotmat, location, draw);

        (Vector2[] points, uint _, float _) = Assert.Single(draw.DrawnPolygons);
        Assert.Equal(2, points.Length);

        AssertVector2AlmostEqual(expectedA, points[0]);
        AssertVector2AlmostEqual(expectedB, points[1]);
    }

    [Fact]
    public void DrawLasersProjectsAlongFiringDirectionAndClipsToViewBoundary()
    {
        // Arrange: a laser mount offset up-and-left of the ship's nose, on a ship
        // sitting to the right of centre, so the bolt's real trajectory exits
        // through the top of the view rather than the fixed screen-edge X /
        // fully-random Y the previous (buggy) implementation always produced.
        Vector4 location = new(500, 0, 1000, 0);
        Vector4 mountCoords = new(10, 20, 50, 0);
        Point mountPoint = new() { Coords = mountCoords, FaceNormals = [] };

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new FakeRandomSource { RandomValue = 0 }))
        {
            Rotmat = Matrix4x4.Identity,
            Location = location,
            LaserFront = 0,
            Flags = ShipProperties.Firing,
            Model = new()
            {
                FaceNormals = [],
                Faces = [],
                Lines = [],
                Points = [mountPoint],
            },
        };

        // Act
        ship.Draw();

        // Assert: replicate the production projection to get the expected mount
        // and (far-distance) aim points, then the point where that ray leaves the
        // view rectangle (FakeEliteDraw: Left=0, Right=511, Top=0, Bottom=511).
        Vector2 expectedMount = Project(mountCoords, location, draw);
        Vector2 expectedAim = Project(mountCoords * 1_000_000f, location, draw);
        Vector2 direction = expectedAim - expectedMount;

        float exitViaLeft = (draw.Left - expectedMount.X) / direction.X;
        float exitViaTop = (draw.Top - expectedMount.Y) / direction.Y;
        float exitDistance = MathF.Min(exitViaLeft, exitViaTop);
        Vector2 expectedEnd = expectedMount + (direction * exitDistance);

        (Vector2[] points, uint _, float _) = Assert.Single(draw.DrawnPolygons);
        Assert.Equal(2, points.Length);
        AssertVector2AlmostEqual(expectedMount, points[0]);
        AssertVector2AlmostEqual(expectedEnd, points[1]);

        // The old code always picked X = 0 or 511 (whichever screen edge is
        // opposite the ship) regardless of geometry; here the ray actually exits
        // through the top edge, so X lands well away from either edge.
        Assert.True(MathF.Abs(expectedEnd.X) > 1f);
        Assert.True(MathF.Abs(expectedEnd.X - 511) > 1f);
    }

    private static Vector2 Project(Vector4 localCoords, Vector4 location, FakeEliteDraw draw)
    {
        Vector4 vec = localCoords + location;
        if (vec.Z <= 0)
        {
            vec.Z = 1;
        }

        float x = ((vec.X * 256 / vec.Z) + (draw.Centre.X / 2)) * draw.Graphics.Scale;
        float y = ((-vec.Y * 256 / vec.Z) + (draw.Centre.Y / 2)) * draw.Graphics.Scale;
        return new(x, y);
    }

    private static Vector2 ProjectUsingRotmatBasis(Vector4 point, Matrix4x4 rotmat, Vector4 location, FakeEliteDraw draw)
    {
        Vector4 vec = (rotmat.GetRow(0) * point.X) + (rotmat.GetRow(1) * point.Y) + (rotmat.GetRow(2) * point.Z);
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
