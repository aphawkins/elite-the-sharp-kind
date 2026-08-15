// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using SharpKind.Assets.Models;
using SharpKind.Graphics.Rendering;

namespace SharpKind.Graphics.Tests;

public class ModelGeometryTests
{
    private const float Tolerance = 0.0001f;

    // A triangle in the z = 0 plane, and a smaller one sitting in that same
    // plane - a decal on the face beneath it.
    private static readonly Vector4[] s_decalPoints =
    [
        new(-100, -100, 0, 0),
        new(100, -100, 0, 0),
        new(0, 100, 0, 0),
        new(-10, -10, 0, 0),
        new(10, -10, 0, 0),
        new(0, 10, 0, 0),
    ];

    [Fact]
    public void AFaceOnNoEarlierPlaneRootsToItself()
    {
        ThreeDModel model = BuildModel(
            [new(0, 0, 0, 0), new(1, 0, 0, 0), new(0, 1, 0, 0), new(0, 0, 1, 0)],
            [[0, 1, 2], [0, 1, 3]]);

        ModelGeometry geometry = ModelGeometry.For(model);

        Assert.Equal(0, geometry.FaceRoots[0]);
        Assert.Equal(1, geometry.FaceRoots[1]);
    }

    [Fact]
    public void AFaceLyingInAnEarlierFacesPlaneRootsToIt()
    {
        ModelGeometry geometry = ModelGeometry.For(BuildModel(s_decalPoints, [[0, 2, 1], [3, 5, 4]]));

        Assert.Equal(0, geometry.FaceRoots[0]);
        Assert.Equal(0, geometry.FaceRoots[1]);
    }

    // A detail line has no three points to cross, so it takes the normal of
    // the face it lies on - which is what culls it with that face.
    [Fact]
    public void ATwoPointDetailLineTakesItsRootFacesNormal()
    {
        ModelGeometry geometry = ModelGeometry.For(BuildModel(s_decalPoints, [[0, 2, 1], [3, 4]]));

        Assert.Equal(0, geometry.FaceRoots[1]);
        AssertAlmostEqual(geometry.FaceNormals[0], geometry.FaceNormals[1]);
    }

    [Fact]
    public void EachFaceGetsItsOwnUnitNormal()
    {
        ThreeDModel model = BuildModel(
            [new(-100, -100, 0, 0), new(100, -100, 0, 0), new(0, 100, 0, 0)],
            [[0, 2, 1]]);

        ModelGeometry geometry = ModelGeometry.For(model);

        AssertAlmostEqual(new(0, 0, -1), geometry.FaceNormals[0]);
    }

    // A decal lies in the plane of the face beneath it, so counting its normal
    // too would weight that plane twice at every corner it touches.
    [Fact]
    public void CornerNormalsExcludeADecalFromTheAveraging()
    {
        ModelGeometry geometry = ModelGeometry.For(BuildModel(s_decalPoints, [[0, 2, 1], [3, 5, 4]]));

        Assert.All(geometry.CornerNormals[0], n => AssertAlmostEqual(new(0, 0, -1), n));
        Assert.All(geometry.CornerNormals[1], n => Assert.Equal(Vector3.Zero, n));
    }

    [Fact]
    public void TheBoundingRadiusReachesTheFurthestPoint()
    {
        ThreeDModel model = BuildModel(
            [new(3, 4, 0, 0), new(1, 0, 0, 0), new(0, 0, 2, 0)],
            [[0, 1, 2]]);

        Assert.Equal(5f, ModelGeometry.For(model).BoundingRadius, Tolerance);
    }

    [Fact]
    public void TheFullyLitValueComesFromEveryFacesColour()
    {
        FastColor dim = new(255, 10, 20, 30);
        FastColor bright = new(255, 40, 90, 5);

        ThreeDModel model = BuildModel(
            [new(0, 0, 0, 0), new(1, 0, 0, 0), new(0, 1, 0, 0), new(0, 0, 1, 0)],
            [[0, 1, 2], [0, 1, 3]]);
        model.Faces[0].Color = dim;
        model.Faces[1].Color = bright;

        Assert.Equal(LambertShading.FullyLit([dim, bright]), ModelGeometry.For(model).FullyLit);
    }

    // Every instance drawn from one model asks the same questions of it, and
    // the answers cannot change while it is loaded.
    [Fact]
    public void TheSameModelIsAnalysedOnlyOnce()
    {
        ThreeDModel model = BuildModel([new(0, 0, 0, 0), new(1, 0, 0, 0), new(0, 1, 0, 0)], [[0, 1, 2]]);

        Assert.Same(ModelGeometry.For(model), ModelGeometry.For(model));
    }

    [Fact]
    public void DifferentModelsGetTheirOwnAnalysis()
    {
        Vector4[] points = [new(0, 0, 0, 0), new(1, 0, 0, 0), new(0, 1, 0, 0)];

        Assert.NotSame(
            ModelGeometry.For(BuildModel(points, [[0, 1, 2]])),
            ModelGeometry.For(BuildModel(points, [[0, 1, 2]])));
    }

    private static void AssertAlmostEqual(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.X, actual.X, Tolerance);
        Assert.Equal(expected.Y, actual.Y, Tolerance);
        Assert.Equal(expected.Z, actual.Z, Tolerance);
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
}
