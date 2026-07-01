// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;
using Useful.Maths;
using Xunit;

namespace Useful.Tests;

public class VectorMathsTests
{
    private const float Tolerance = 1e-5f;

    [Fact]
    public void GetLeftHandedBasisMatrixReturnsExpectedValues()
    {
        // Act
        Matrix4x4 m = VectorMaths.GetLeftHandedBasisMatrix;

        // Assert
        AssertVectorAlmostEqual(new Vector4(1, 0, 0, 0), m.GetRow(0));
        AssertVectorAlmostEqual(new Vector4(0, 1, 0, 0), m.GetRow(1));
        AssertVectorAlmostEqual(new Vector4(0, 0, -1, 0), m.GetRow(2));
        AssertVectorAlmostEqual(new Vector4(0, 0, 0, 0), m.GetRow(3));
    }

    [Fact]
    public void MultiplyVectorAppliesMatrixCorrectly()
    {
        // Arrange
        Vector4 v = new(1, 2, 3, 0);

        // Act -> third row negates Z
        Vector4 result = Vector4.Transform(v, VectorMaths.GetLeftHandedBasisMatrix);

        // Assert -> expected (1,2,-3)
        AssertVectorAlmostEqual(new Vector4(1, 2, -3, 0), result);
    }

    [Fact]
    public void GetRowReturnsCorrectRow()
    {
        // Arrange: every component distinct, so any mixed-up mapping is caught.
        Matrix4x4 m = new(
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16);

        // Act & Assert
        AssertVectorAlmostEqual(new Vector4(1, 2, 3, 4), m.GetRow(0));
        AssertVectorAlmostEqual(new Vector4(5, 6, 7, 8), m.GetRow(1));
        AssertVectorAlmostEqual(new Vector4(9, 10, 11, 12), m.GetRow(2));
        AssertVectorAlmostEqual(new Vector4(13, 14, 15, 16), m.GetRow(3));
    }

    [Fact]
    public void GetRowThrowsForInvalidRow()
    {
        // Arrange
        Matrix4x4 m = Matrix4x4.Identity;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => m.GetRow(4));
    }

    [Fact]
    public void WithRowReplacesOnlyTheGivenRow()
    {
        // Arrange
        Matrix4x4 m = Matrix4x4.Identity;

        // Act
        Matrix4x4 result = m.WithRow(2, new Vector4(9, 10, 11, 12));

        // Assert: the requested row changed...
        AssertVectorAlmostEqual(new Vector4(9, 10, 11, 12), result.GetRow(2));

        // ...the others didn't...
        AssertVectorAlmostEqual(new Vector4(1, 0, 0, 0), result.GetRow(0));
        AssertVectorAlmostEqual(new Vector4(0, 1, 0, 0), result.GetRow(1));
        AssertVectorAlmostEqual(new Vector4(0, 0, 0, 1), result.GetRow(3));

        // ...and the original matrix (a value type) is untouched.
        AssertVectorAlmostEqual(new Vector4(0, 0, 1, 0), m.GetRow(2));
    }

    [Fact]
    public void WithRowThrowsForInvalidRow()
    {
        // Arrange
        Matrix4x4 m = Matrix4x4.Identity;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => m.WithRow(4, Vector4.Zero));
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
        Matrix4x4 matrix = new(
            1,
            2,
            3,
            0,
            -1,
            4,
            0.5f,
            0,
            0.1f,
            -0.3f,
            2.0f,
            0,
            0,
            0,
            0,
            0);

        Matrix4x4 result = VectorMaths.RotateVector(matrix, 0, 0);

        AssertVectorAlmostEqual(matrix.GetRow(0), result.GetRow(0));
        AssertVectorAlmostEqual(matrix.GetRow(1), result.GetRow(1));
        AssertVectorAlmostEqual(matrix.GetRow(2), result.GetRow(2));
    }

    [Fact]
    public void RotateVectorChangesValuesForNonZeroAngles()
    {
        Matrix4x4 matrix = new(
            1,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0);

        Matrix4x4 result = VectorMaths.RotateVector(matrix, 0.1f, 0.2f);

        // Should not be identical to input for non-zero angles
        Vector4 row0 = matrix.GetRow(0);
        Vector4 row1 = matrix.GetRow(1);
        Vector4 row2 = matrix.GetRow(2);
        Vector4 resultRow0 = result.GetRow(0);
        Vector4 resultRow1 = result.GetRow(1);
        Vector4 resultRow2 = result.GetRow(2);

        bool anyDifferent =
            Math.Abs(resultRow0.X - row0.X) > Tolerance ||
                Math.Abs(resultRow0.Y - row0.Y) > Tolerance ||
                Math.Abs(resultRow0.Z - row0.Z) > Tolerance ||
                Math.Abs(resultRow1.X - row1.X) > Tolerance ||
                Math.Abs(resultRow1.Y - row1.Y) > Tolerance ||
                Math.Abs(resultRow1.Z - row1.Z) > Tolerance ||
                Math.Abs(resultRow2.X - row2.X) > Tolerance ||
                Math.Abs(resultRow2.Y - row2.Y) > Tolerance ||
                Math.Abs(resultRow2.Z - row2.Z) > Tolerance;

        Assert.True(anyDifferent, "RotateVector should modify the vectors when angles are non-zero.");
    }

    [Fact]
    public void TidyMatrixProducesOrthogonalUnitVectors()
    {
        // Construct a deliberately messy matrix
        Matrix4x4 mat = new(
            0.2f,
            0.9f,
            0.1f,
            0,
            0.9f,
            -0.1f,
            0.3f,
            0,
            0.3f,
            0.4f,
            0.7f,
            0,
            0,
            0,
            0,
            0);

        // Act
        Matrix4x4 result = VectorMaths.OrthonormalizeBasis(mat);
        Vector4 row0 = result.GetRow(0);
        Vector4 row1 = result.GetRow(1);
        Vector4 row2 = result.GetRow(2);

        // Assert: each vector should be unit length
        Assert.InRange(row0.Length(), 1f - Tolerance, 1f + Tolerance);
        Assert.InRange(row1.Length(), 1f - Tolerance, 1f + Tolerance);
        Assert.InRange(row2.Length(), 1f - Tolerance, 1f + Tolerance);

        // Assert: orthogonality (dot products near zero)
        float d01 = Vector4.Dot(row0, row1);
        float d12 = Vector4.Dot(row1, row2);
        float d20 = Vector4.Dot(row2, row0);
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
