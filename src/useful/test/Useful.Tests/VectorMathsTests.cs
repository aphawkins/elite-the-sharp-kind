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

        Vector4[] copy = [matrix[0], matrix[1], matrix[2], matrix[3]];

        Vector4[] result = VectorMaths.RotateVector(copy, 0, 0);

        AssertVectorAlmostEqual(matrix[0], result[0]);
        AssertVectorAlmostEqual(matrix[1], result[1]);
        AssertVectorAlmostEqual(matrix[2], result[2]);
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

        Vector4[] input = [matrix[0], matrix[1], matrix[2], matrix[3]];
        Vector4[] result = VectorMaths.RotateVector(input, 0.1f, 0.2f);

        // Should not be identical to input for non-zero angles
        bool anyDifferent =
            Math.Abs(result[0].X - matrix[0].X) > Tolerance ||
                Math.Abs(result[0].Y - matrix[0].Y) > Tolerance ||
                Math.Abs(result[0].Z - matrix[0].Z) > Tolerance ||
                Math.Abs(result[1].X - matrix[1].X) > Tolerance ||
                Math.Abs(result[1].Y - matrix[1].Y) > Tolerance ||
                Math.Abs(result[1].Z - matrix[1].Z) > Tolerance ||
                Math.Abs(result[2].X - matrix[2].X) > Tolerance ||
                Math.Abs(result[2].Y - matrix[2].Y) > Tolerance ||
                Math.Abs(result[2].Z - matrix[2].Z) > Tolerance;

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
