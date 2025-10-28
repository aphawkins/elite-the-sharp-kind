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
        Vector3[] m1 = VectorMaths.GetInitialMatrix();
        Vector3[] m2 = VectorMaths.GetInitialMatrix();

        // Assert - values
        AssertVectorAlmostEqual(new Vector3(1, 0, 0), m1[0]);
        AssertVectorAlmostEqual(new Vector3(0, 1, 0), m1[1]);
        AssertVectorAlmostEqual(new Vector3(0, 0, -1), m1[2]);

        // Assert - clones (different array instances)
        Assert.False(ReferenceEquals(m1, m2));

        // and different elements instances (structs so reference equality not applicable), ensure modifying one doesn't affect the other
        m1[0] = new Vector3(9, 9, 9);
        AssertVectorAlmostEqual(new Vector3(1, 0, 0), m2[0]);
    }

    [Fact]
    public void MultiplyVectorAppliesMatrixCorrectly()
    {
        // Arrange
        Vector3[] mat = VectorMaths.GetInitialMatrix(); // third row negates Z
        Vector3 v = new(1, 2, 3);

        // Act
        Vector3 result = VectorMaths.MultiplyVector(v, mat);

        // Assert -> expected (1,2,-3)
        AssertVectorAlmostEqual(new Vector3(1, 2, -3), result);
    }

    [Fact]
    public void UnitVectorReturnsNormalizedVector()
    {
        Vector3 v = new(2f, 0f, 0f);
        Vector3 unit = VectorMaths.UnitVector(v);
        AssertVectorAlmostEqual(new Vector3(1f, 0f, 0f), unit);
    }

    [Fact]
    public void VectorDotProductComputesCosineLikeDot()
    {
        Vector3 a = new(1f, 0f, 0f);
        Vector3 b = new(0f, 1f, 0f);
        float dot = VectorMaths.VectorDotProduct(a, b);
        Assert.InRange(dot, -Tolerance, Tolerance); // near zero

        Vector3 c = new(2f, 0f, 0f);
        dot = VectorMaths.VectorDotProduct(a, c);
        Assert.InRange(dot, 2f - Tolerance, 2f + Tolerance);
    }

    [Fact]
    public void RotateVectorWithZeroAnglesIsIdentity()
    {
        Vector3[] matrix =
        [
            new Vector3(1f, 2f, 3f),
            new Vector3(-1f, 4f, 0.5f),
            new Vector3(0.1f, -0.3f, 2.0f)
        ];

        Vector3[] copy = [matrix[0], matrix[1], matrix[2]];

        Vector3[] result = VectorMaths.RotateVector(copy, 0f, 0f);

        AssertVectorAlmostEqual(matrix[0], result[0]);
        AssertVectorAlmostEqual(matrix[1], result[1]);
        AssertVectorAlmostEqual(matrix[2], result[2]);
    }

    [Fact]
    public void RotateVectorChangesValuesForNonZeroAngles()
    {
        Vector3[] matrix =
        [
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f)
        ];

        Vector3[] input = [matrix[0], matrix[1], matrix[2]];
        Vector3[] result = VectorMaths.RotateVector(input, 0.1f, 0.2f);

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
        Vector3[] mat =
        [
            new Vector3(0.2f, 0.9f, 0.1f),
            new Vector3(0.9f, -0.1f, 0.3f),
            new Vector3(0.3f, 0.4f, 0.7f),
        ];

        // Act
        VectorMaths.TidyMatrix(mat);

        // Assert: each vector should be unit length
        float len0 = mat[0].Length();
        float len1 = mat[1].Length();
        float len2 = mat[2].Length();
        Assert.InRange(len0, 1f - Tolerance, 1f + Tolerance);
        Assert.InRange(len1, 1f - Tolerance, 1f + Tolerance);
        Assert.InRange(len2, 1f - Tolerance, 1f + Tolerance);

        // Assert: orthogonality (dot products near zero)
        float d01 = Vector3.Dot(mat[0], mat[1]);
        float d12 = Vector3.Dot(mat[1], mat[2]);
        float d20 = Vector3.Dot(mat[2], mat[0]);
        Assert.InRange(d01, -Tolerance, Tolerance);
        Assert.InRange(d12, -Tolerance, Tolerance);
        Assert.InRange(d20, -Tolerance, Tolerance);
    }

    private static void AssertVectorAlmostEqual(Vector3 expected, Vector3 actual, float tol = Tolerance)
    {
        Assert.InRange(actual.X, expected.X - tol, expected.X + tol);
        Assert.InRange(actual.Y, expected.Y - tol, expected.Y + tol);
        Assert.InRange(actual.Z, expected.Z - tol, expected.Z + tol);
    }
}
