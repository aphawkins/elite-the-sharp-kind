// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Maths;

public static class VectorMaths
{
    // Left-handed basis matrix.
    public static Matrix4x4 GetLeftHandedBasisMatrix => new(
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
        -1,
        0,
        0,
        0,
        0,
        0);

    /// <summary>
    /// Rotate a 4x4 matrix by two angles.
    /// The rotation is applied to the matrix' basis vectors: result = r * matrix,
    /// where R is the rotation composed of a rotation about Z by <paramref name="alpha"/>
    /// followed by a rotation about X by <paramref name="beta"/>.
    /// </summary>
    public static Matrix4x4 RotateVector(Matrix4x4 matrix, float alpha, float beta)
    {
        // Rotation about Z (XY plane), then about X (YZ plane).
        Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
        Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);

        // Compose: apply rotZ then rotX => r = rotX * rotZ
        Matrix4x4 r = Matrix4x4.Multiply(rotX, rotZ);

        // Apply rotation to the matrix basis vectors: r * matrix
        return Matrix4x4.Multiply(r, matrix);
    }

    public static void TidyMatrix(Vector4[] mat)
    {
        Guard.ArgumentNull(mat);

        mat[2] = UnitVector(mat[2]);

        if (mat[2].X is > -1 and < 1)
        {
            if (mat[2].Y is > -1 and < 1)
            {
                mat[1].Z = -((mat[2].X * mat[1].X) + (mat[2].Y * mat[1].Y)) / mat[2].Z;
            }
            else
            {
                mat[1].Y = -((mat[2].X * mat[1].X) + (mat[2].Z * mat[1].Z)) / mat[2].Y;
            }
        }
        else
        {
            mat[1].X = -((mat[2].Y * mat[1].Y) + (mat[2].Z * mat[1].Z)) / mat[2].X;
        }

        mat[1] = UnitVector(mat[1]);

        // xyzzy... nothing happens.
        mat[0].X = (mat[1].Y * mat[2].Z) - (mat[1].Z * mat[2].Y);
        mat[0].Y = (mat[1].Z * mat[2].X) - (mat[1].X * mat[2].Z);
        mat[0].Z = (mat[1].X * mat[2].Y) - (mat[1].Y * mat[2].X);
    }

    /// <summary>
    /// Convert a vector into a vector of unit (1) length.
    /// </summary>
    public static Vector4 UnitVector(Vector4 vec)
    {
        Vector3 unit = Vector3.Divide(vec.AsVector3(), vec.AsVector3().Length());
        return new(unit, vec.W);
    }

    /// <summary>
    /// Calculate the dot product of two vectors sharing a common point.
    /// </summary>
    /// <returns>The cosine of the angle between the two vectors.</returns>
    public static float VectorDotProduct(Vector4 first, Vector4 second) => Vector4.Dot(first, second);
}
