// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;
using Useful.Maths;
using Xunit;

namespace Useful.Tests;

public class VectorMathsTests
{
    private const float Tolerance = 1e-5f;

    [Fact]
    public void GetInitialMatrixReturnsExpectedValuesAndIsCloned()
    {
        // Act
        Vector4[] m1 = VectorMaths.GetLeftHandedBasisMatrix.ToVector4Array();
        Vector4[] m2 = VectorMaths.GetLeftHandedBasisMatrix.ToVector4Array();

        // Assert - values
        AssertVectorAlmostEqual(new Vector4(1, 0, 0, 0), m1[0]);
        AssertVectorAlmostEqual(new Vector4(0, 1, 0, 0), m1[1]);
        AssertVectorAlmostEqual(new Vector4(0, 0, -1, 0), m1[2]);
        AssertVectorAlmostEqual(new Vector4(0, 0, 0, 0), m1[3]);

        // Assert - clones (different array instances)
        Assert.False(ReferenceEquals(m1, m2));

        // and different elements instances (structs so reference equality not applicable), ensure modifying one doesn't affect the other
        m1[0] = new Vector4(9, 9, 9, 0);
        AssertVectorAlmostEqual(new Vector4(1, 0, 0, 0), m2[0]);
    }

    [Fact]
    public void MultiplyVectorAppliesMatrixCorrectly()
    {
        // Arrange
        Vector4[] mat = VectorMaths.GetLeftHandedBasisMatrix.ToVector4Array(); // third row negates Z
        Vector4 v = new(1, 2, 3, 0);

        // Act
        Vector4 result = Vector4.Transform(v, mat.ToMatrix4x4());

        // Assert -> expected (1,2,-3)
        AssertVectorAlmostEqual(new Vector4(1, 2, -3, 0), result);
    }

    [Fact]
    public void ToMatrix4x4MapsEachVectorComponentToItsOwnColumn()
    {
        // Arrange: every component distinct, so any mixed-up mapping is caught.
        Vector4[] vecs =
        [
            new(1, 2, 3, 4),
            new(5, 6, 7, 8),
            new(9, 10, 11, 12),
            new(13, 14, 15, 16),
        ];

        // Act
        Matrix4x4 result = vecs.ToMatrix4x4();

        // Assert: column i of the matrix must equal vecs[i], including the fourth vector -
        // regression test for a bug where M24 read from vecs[2].Y instead of vecs[3].Y.
        AssertVectorAlmostEqual(vecs[0], new(result.M11, result.M21, result.M31, result.M41));
        AssertVectorAlmostEqual(vecs[1], new(result.M12, result.M22, result.M32, result.M42));
        AssertVectorAlmostEqual(vecs[2], new(result.M13, result.M23, result.M33, result.M43));
        AssertVectorAlmostEqual(vecs[3], new(result.M14, result.M24, result.M34, result.M44));
    }

    [Fact]
    public void UnitVectorReturnsNormalizedVector()
    {
        Vector4 v = new(2, 0, 0, 0);
        Vector4 unit = VectorMaths.UnitVector(v);
        AssertVectorAlmostEqual(new Vector4(1, 0, 0, 0), unit);
    }

    [Fact]
    public void VectorDotProductComputesCosineLikeDot()
    {
        Vector4 a = new(1, 0, 0, 0);
        Vector4 b = new(0, 1, 0, 0);
        float dot = VectorMaths.VectorDotProduct(a, b);
        Assert.InRange(dot, -Tolerance, Tolerance); // near zero

        Vector4 c = new(2, 0, 0, 0);
        dot = VectorMaths.VectorDotProduct(a, c);
        Assert.InRange(dot, 2f - Tolerance, 2f + Tolerance);
    }

    [Fact]
    public void RotateVectorWithZeroAnglesIsIdentity()
    {
        Vector4[] matrix =
        [
            new Vector4(1, 2, 3, 0),
            new Vector4(-1, 4, 0.5f, 0),
            new Vector4(0.1f, -0.3f, 2.0f, 0),
            new Vector4(0, 0, 0, 0)
        ];

        Matrix4x4 result = VectorMaths.RotateVector(matrix.ToMatrix4x4(), 0, 0);
        Vector4[] resultVecs = result.ToVector4Array();

        AssertVectorAlmostEqual(matrix[0], resultVecs[0]);
        AssertVectorAlmostEqual(matrix[1], resultVecs[1]);
        AssertVectorAlmostEqual(matrix[2], resultVecs[2]);
    }

    [Fact]
    public void RotateVectorChangesValuesForNonZeroAngles()
    {
        Vector4[] matrix =
        [
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 0)
        ];

        Matrix4x4 result = VectorMaths.RotateVector(matrix.ToMatrix4x4(), 0.1f, 0.2f);
        Vector4[] resultVecs = result.ToVector4Array();

        // Should not be identical to input for non-zero angles
        bool anyDifferent =
            Math.Abs(resultVecs[0].X - matrix[0].X) > Tolerance ||
                Math.Abs(resultVecs[0].Y - matrix[0].Y) > Tolerance ||
                Math.Abs(resultVecs[0].Z - matrix[0].Z) > Tolerance ||
                Math.Abs(resultVecs[1].X - matrix[1].X) > Tolerance ||
                Math.Abs(resultVecs[1].Y - matrix[1].Y) > Tolerance ||
                Math.Abs(resultVecs[1].Z - matrix[1].Z) > Tolerance ||
                Math.Abs(resultVecs[2].X - matrix[2].X) > Tolerance ||
                Math.Abs(resultVecs[2].Y - matrix[2].Y) > Tolerance ||
                Math.Abs(resultVecs[2].Z - matrix[2].Z) > Tolerance;

        Assert.True(anyDifferent, "RotateVector should modify the vectors when angles are non-zero.");
    }

    [Fact]
    public void TidyMatrixProducesOrthogonalUnitVectors()
    {
        // Construct a deliberately messy matrix
        Vector4[] mat =
        [
            new Vector4(0.2f, 0.9f, 0.1f, 0),
            new Vector4(0.9f, -0.1f, 0.3f, 0),
            new Vector4(0.3f, 0.4f, 0.7f, 0),
            new Vector4(0, 0, 0, 0),
        ];

        // Act
        mat = VectorMaths.OrthonormalizeBasis(mat.ToMatrix4x4()).ToVector4Array();

        // Assert: each vector should be unit length
        float len0 = mat[0].Length();
        float len1 = mat[1].Length();
        float len2 = mat[2].Length();
        Assert.InRange(len0, 1f - Tolerance, 1f + Tolerance);
        Assert.InRange(len1, 1f - Tolerance, 1f + Tolerance);
        Assert.InRange(len2, 1f - Tolerance, 1f + Tolerance);

        // Assert: orthogonality (dot products near zero)
        float d01 = Vector4.Dot(mat[0], mat[1]);
        float d12 = Vector4.Dot(mat[1], mat[2]);
        float d20 = Vector4.Dot(mat[2], mat[0]);
        Assert.InRange(d01, -Tolerance, Tolerance);
        Assert.InRange(d12, -Tolerance, Tolerance);
        Assert.InRange(d20, -Tolerance, Tolerance);
    }

    private static void AssertVectorAlmostEqual(Vector4 expected, Vector4 actual, float tol = Tolerance)
    {
        Assert.InRange(actual.X, expected.X - tol, expected.X + tol);
        Assert.InRange(actual.Y, expected.Y - tol, expected.Y + tol);
        Assert.InRange(actual.Z, expected.Z - tol, expected.Z + tol);
    }
}
