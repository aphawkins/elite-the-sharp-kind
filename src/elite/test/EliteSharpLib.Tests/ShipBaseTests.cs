// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using Useful;
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

        // A 2-point "face" lying on no other face's plane has no normal to cull
        // against, so it always passes the visibility check in DrawModelFaces,
        // keeping this test independent of it.
        Face face = new() { Color = default, Points = [modelPointA, modelPointB], PointIndices = [0, 1] };

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

        (Vector2[] points, float[] _, FastColor _, float _) = Assert.Single(draw.DrawnPolygons);
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

        float exitViaLeft = (draw.Layout.ViewportLeft - expectedMount.X) / direction.X;
        float exitViaTop = (draw.Layout.ViewportTop - expectedMount.Y) / direction.Y;
        float exitDistance = MathF.Min(exitViaLeft, exitViaTop);
        Vector2 expectedEnd = expectedMount + (direction * exitDistance);

        (Vector2[] points, float[] _, FastColor _, float _) = Assert.Single(draw.DrawnPolygons);
        Assert.Equal(2, points.Length);
        AssertVector2AlmostEqual(expectedMount, points[0]);
        AssertVector2AlmostEqual(expectedEnd, points[1]);

        // The old code always picked X = 0 or 511 (whichever screen edge is
        // opposite the ship) regardless of geometry; here the ray actually exits
        // through the top edge, so X lands well away from either edge.
        Assert.True(MathF.Abs(expectedEnd.X) > 1f);
        Assert.True(MathF.Abs(expectedEnd.X - 511) > 1f);
    }

    [Fact]
    public void DrawSubmitsTheCameraDepthOfEachPointOfATiltedFace()
    {
        // Arrange: a triangle steeply angled to the camera, so its three
        // points sit at three clearly different depths. Submitting one flat
        // depth for all three - as the renderer used to - interpolates a
        // constant across the face and defeats the per-pixel depth test.
        Vector4 location = new(0, 0, 1000, 0);
        Vector4 pointA = new(-100, -100, 0, 0);
        Vector4 pointB = new(100, -100, 300, 0);
        Vector4 pointC = new(0, 100, 600, 0);

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new Random(0)))
        {
            Rotmat = Matrix4x4.Identity,
            Location = location,

            // Wound so the face survives the backface cull.
            Model = BuildModel([pointA, pointB, pointC], [[0, 2, 1]]),
        };

        // Act
        ship.Draw();

        // Assert
        (Vector2[] points, float[] depths, FastColor _, float _) = Assert.Single(draw.DrawnPolygons);
        Assert.Equal(3, points.Length);
        Assert.Equal([1000f, 1600f, 1300f], depths);
    }

    [Fact]
    public void DrawBiasesADecalNearerThanTheFaceItSitsOn()
    {
        // Arrange: a small triangle exactly in the plane of a larger one, as
        // a cockpit window sits in the plane of the hull. Per-vertex depth
        // makes the two tie pixel for pixel, so the decal needs a nudge
        // towards the camera to render over its base face.
        Vector4 location = new(0, 0, 1000, 0);
        Vector4[] points =
        [
            new(-100, -100, 0, 0),
            new(100, -100, 0, 0),
            new(0, 100, 0, 0),
            new(-10, -10, 0, 0),
            new(10, -10, 0, 0),
            new(0, 10, 0, 0),
        ];

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new Random(0)))
        {
            Rotmat = Matrix4x4.Identity,
            Location = location,

            // Wound so both faces survive the backface cull.
            Model = BuildModel(points, [[0, 2, 1], [3, 5, 4]]),
        };

        // Act
        ship.Draw();

        // Assert
        Assert.Equal(2, draw.DrawnPolygons.Count);
        Assert.Equal([1000f, 1000f, 1000f], draw.DrawnPolygons[0].Depths);
        Assert.All(draw.DrawnPolygons[1].Depths, d => Assert.InRange(d, 990f, 999.9f));

        // The decal keeps its base face's whole-face key, so the painter's
        // strategy still ties and draws it later.
        Assert.Equal(draw.DrawnPolygons[0].Z, draw.DrawnPolygons[1].Z);
    }

    [Fact]
    public void DrawKeepsAFrontFacingFaceThatStraddlesTheCameraPlane()
    {
        // Arrange: a triangle with one point behind the camera plane and two
        // well in front of it. The projection clamps the behind point's depth
        // to 1, which puts it on screen at the view centre instead of off
        // behind the viewer - so a winding test run on the projected outline
        // decides on meaningless coordinates and culls this face, even though
        // most of it is in front of the camera and facing it. Culling in
        // camera space, before the clamp can touch anything, keeps it.
        Vector4 pointA = new(0, 0, -100, 0);
        Vector4 pointB = new(100, 0, 100, 0);
        Vector4 pointC = new(0, 100, 100, 0);

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new Random(0)))
        {
            Rotmat = Matrix4x4.Identity,
            Location = Vector4.Zero,
            Model = BuildModel([pointA, pointB, pointC], [[0, 1, 2]]),
        };

        // Act
        ship.Draw();

        // Assert: drawn, and clipped to the near plane rather than including
        // the behind-camera point.
        (Vector2[] points, float[] depths, FastColor _, float _) = Assert.Single(draw.DrawnPolygons);
        Assert.Equal(4, points.Length);
        Assert.All(depths, d => Assert.True(d > 0));
    }

    [Fact]
    public void DrawCullsADetailLineLyingOnABackFacingFace()
    {
        // Arrange: a back-facing triangle with a 2-point detail line in its
        // plane, as hull detail sits on a hull face. The line has no winding
        // of its own, so it used to pass every frame whichever way it faced,
        // leaving a stray stub poking off the far side of the silhouette.
        Vector4 location = new(0, 0, 1000, 0);
        Vector4[] points =
        [
            new(-100, -100, 0, 0),
            new(100, -100, 0, 0),
            new(0, 100, 0, 0),
            new(-10, -10, 0, 0),
            new(10, -10, 0, 0),
        ];

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new Random(0)))
        {
            Rotmat = Matrix4x4.Identity,
            Location = location,
            Model = BuildModel(points, [[0, 1, 2], [3, 4]]),
        };

        // Act
        ship.Draw();

        // Assert
        Assert.Empty(draw.DrawnPolygons);
    }

    // A detail line lying on no other face's plane has no root to inherit a
    // normal from. The model still records which faces each vertex belongs
    // to, so the faces the line runs along are those its two ends share.
    [Theory]
    [InlineData(0, 0, 1, false)]
    [InlineData(0, 0, -1, true)]
    public void DrawCullsAnUnrootedDetailLineByTheNormalItsEndsShare(
        float normalX,
        float normalY,
        float normalZ,
        bool expectDrawn)
    {
        // Arrange
        FaceNormal shared = new() { Direction = new(normalX, normalY, normalZ, 0), Visible = true };
        Point start = new() { Coords = new(-10, 0, 0, 0), FaceNormals = [shared] };
        Point end = new() { Coords = new(10, 0, 0, 0), FaceNormals = [shared] };

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new Random(0)))
        {
            Rotmat = Matrix4x4.Identity,
            Location = new(0, 0, 1000, 0),
            Model = new()
            {
                FaceNormals = [shared],
                Faces = [new() { Color = default, Points = [start, end], PointIndices = [0, 1] }],
                Lines = [],
                Points = [start, end],
            },
        };

        // Act
        ship.Draw();

        // Assert
        Assert.Equal(expectDrawn, draw.DrawnPolygons.Count == 1);
    }

    // The bolt springs from a mount on the hull, so it is only visible when
    // that part of the hull is. It bypasses the face loop entirely, so
    // nothing else culls it.
    [Theory]
    [InlineData(0, 0, 1, false)]
    [InlineData(0, 0, -1, true)]
    public void DrawLasersCullsABoltWhoseMountFacesAway(
        float normalX,
        float normalY,
        float normalZ,
        bool expectDrawn)
    {
        // Arrange
        FaceNormal mountNormal = new() { Direction = new(normalX, normalY, normalZ, 0), Visible = true };
        Point mountPoint = new() { Coords = new(10, 20, 50, 0), FaceNormals = [mountNormal] };

        FakeEliteDraw draw = new();
        FakeShip ship = new(draw, new(new FakeRandomSource { RandomValue = 0 }))
        {
            Rotmat = Matrix4x4.Identity,
            Location = new(500, 0, 1000, 0),
            LaserFront = 0,
            Flags = ShipProperties.Firing,
            Model = new()
            {
                FaceNormals = [mountNormal],
                Faces = [],
                Lines = [],
                Points = [mountPoint],
            },
        };

        // Act
        ship.Draw();

        // Assert
        Assert.Equal(expectDrawn, draw.DrawnPolygons.Count == 1);
    }

    private static ThreeDModel BuildModel(Vector4[] coords, int[][] faceIndices)
    {
        Point[] modelPoints = [.. coords.Select(c => new Point { Coords = c, FaceNormals = [] })];

        return new()
        {
            FaceNormals = [],
            Faces =
            [
                .. faceIndices.Select(indices => new Face
                {
                    Color = default,
                    Points = [.. indices.Select(i => modelPoints[i])],
                    PointIndices = [.. indices],
                }),
            ],
            Lines = [],
            Points = [.. modelPoints],
        };
    }

    private static Vector2 Project(Vector4 localCoords, Vector4 location, FakeEliteDraw draw)
    {
        Vector4 vec = localCoords + location;
        if (vec.Z <= 0)
        {
            vec.Z = 1;
        }

        float x = draw.Layout.ViewportCentre.X + (vec.X * draw.Focus / vec.Z);
        float y = draw.Layout.ViewportCentre.Y - (vec.Y * draw.Focus / vec.Z);
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

        float x = draw.Layout.ViewportCentre.X + (vec.X * draw.Focus / vec.Z);
        float y = draw.Layout.ViewportCentre.Y - (vec.Y * draw.Focus / vec.Z);
        return new(x, y);
    }

    private static void AssertVector2AlmostEqual(Vector2 expected, Vector2 actual)
    {
        Assert.InRange(actual.X, expected.X - Tolerance, expected.X + Tolerance);
        Assert.InRange(actual.Y, expected.Y - Tolerance, expected.Y + Tolerance);
    }
}
