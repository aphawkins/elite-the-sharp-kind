// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using SharpKind.Assets.Models;
using SharpKind.Graphics.Rendering;

namespace SharpKind.Graphics.Tests;

public class MeshTransformTests
{
    private const float Tolerance = 0.0001f;

    private static readonly Vector4[] s_unitCorner =
    [
        new(1, 0, 0, 0),
        new(0, 1, 0, 0),
        new(0, 0, 1, 0),
    ];

    [Fact]
    public void AnIdentityFrameAtTheOriginLeavesEveryPointWhereItWas()
    {
        Span<Vector3> placed = stackalloc Vector3[3];

        MeshTransform.TransformPoints(BuildModel(s_unitCorner, [[0, 1, 2]]), Matrix4x4.Identity, Vector3.Zero, placed);

        AssertAlmostEqual(new(1, 0, 0), placed[0]);
        AssertAlmostEqual(new(0, 1, 0), placed[1]);
        AssertAlmostEqual(new(0, 0, 1), placed[2]);
    }

    // The models carry w = 0 coordinates, so a matrix's translation row never
    // reaches them - which is why the translation is its own argument.
    [Fact]
    public void TheTranslationMovesEveryPointEvenThoughTheCoordinatesCarryNoW()
    {
        Matrix4x4 withTranslationRow = Matrix4x4.CreateTranslation(new(100, 200, 300));
        Span<Vector3> placed = stackalloc Vector3[3];

        MeshTransform.TransformPoints(
            BuildModel(s_unitCorner, [[0, 1, 2]]),
            withTranslationRow,
            new(5, 6, 7),
            placed);

        AssertAlmostEqual(new(6, 6, 7), placed[0]);
        AssertAlmostEqual(new(5, 7, 7), placed[1]);
        AssertAlmostEqual(new(5, 6, 8), placed[2]);
    }

    [Fact]
    public void TheFramesRowsAreWhereTheModelsOwnAxesEndUp()
    {
        // A quarter turn about y: the model's x axis ends up on -z, its z axis
        // on x.
        Matrix4x4 frame = Matrix4x4.CreateRotationY(MathF.PI / 2);
        Span<Vector3> placed = stackalloc Vector3[3];

        MeshTransform.TransformPoints(BuildModel(s_unitCorner, [[0, 1, 2]]), frame, Vector3.Zero, placed);

        AssertAlmostEqual(new(0, 0, -1), placed[0]);
        AssertAlmostEqual(new(0, 1, 0), placed[1]);
        AssertAlmostEqual(new(1, 0, 0), placed[2]);
    }

    [Fact]
    public void TransformingIntoTooSmallASpanThrows()
    {
        ThreeDModel model = BuildModel(s_unitCorner, [[0, 1, 2]]);
        Vector3[] tooSmall = new Vector3[2];

        Assert.Throws<ArgumentException>(
            () => MeshTransform.TransformPoints(model, Matrix4x4.Identity, Vector3.Zero, tooSmall));
    }

    [Fact]
    public void AFacesPointsComeBackInTheOrderTheFaceListsThem()
    {
        ThreeDModel model = BuildModel(s_unitCorner, [[2, 0, 1]]);
        Vector3[] placed = [new(1, 0, 0), new(0, 1, 0), new(0, 0, 1)];
        Span<Vector3> face = stackalloc Vector3[3];

        int count = MeshTransform.FacePoints(model, 0, placed, face);

        Assert.Equal(3, count);
        AssertAlmostEqual(new(0, 0, 1), face[0]);
        AssertAlmostEqual(new(1, 0, 0), face[1]);
        AssertAlmostEqual(new(0, 1, 0), face[2]);
    }

    // A buffer sized by MaxFacePoints holds any face of the model, so a caller
    // allocates once rather than per face.
    [Fact]
    public void GatheringIntoTooSmallASpanThrows()
    {
        ThreeDModel model = BuildModel(s_unitCorner, [[0, 1, 2]]);
        Vector3[] placed = [new(1, 0, 0), new(0, 1, 0), new(0, 0, 1)];
        Vector3[] tooSmall = new Vector3[2];

        Assert.Throws<ArgumentException>(() => MeshTransform.FacePoints(model, 0, placed, tooSmall));
    }

    [Fact]
    public void TheLargestFaceSetsTheBufferSize()
    {
        ThreeDModel model = BuildModel(
            [.. s_unitCorner, new(1, 1, 1, 0)],
            [[0, 1], [0, 1, 2, 3], [0, 1, 2]]);

        Assert.Equal(4, MeshTransform.MaxFacePoints(model));
    }

    [Fact]
    public void AModelWithNoFacesNeedsNoBuffer()
        => Assert.Equal(0, MeshTransform.MaxFacePoints(BuildModel(s_unitCorner, [])));

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
