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

    // Right-handed basis matrix.
    public static Matrix4x4 GetRightHandedBasisMatrix => new(
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

    /// <summary>
    /// Rotate a 4x4 matrix's basis vectors by two small angles.
    /// Each basis vector is independently rotated by the same small-angle approximation:
    /// a rotation about Z by <paramref name="alpha"/> followed by a rotation about X by <paramref name="beta"/>.
    /// </summary>
    public static Matrix4x4 RotateVector(Matrix4x4 matrix, float alpha, float beta)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector4 row = matrix.GetRow(i);
            RotateVector(ref row, alpha, beta);
            matrix = matrix.WithRow(i, row);
        }

        return matrix;
    }

    /// <summary>
    /// Tidy a 4x4 basis matrix: orthonormalise basis vectors 0-2 using Gram-Schmidt with safe
    /// fallbacks. Preserves each vector's original W component and leaves vector 3 unchanged.
    /// </summary>
    public static Matrix4x4 OrthonormalizeBasis(Matrix4x4 mat)
    {
        Vector4 side = mat.GetRow(0);
        Vector4 roof = mat.GetRow(1);
        Vector4 nose = mat.GetRow(2);

        Vector3 c1 = new(roof.X, roof.Y, roof.Z);
        Vector3 c2 = new(nose.X, nose.Y, nose.Z);

        // Gram-Schmidt style orthonormalisation with fallbacks for degenerate inputs.

        // Normalize Z basis (c2)
        float len2 = c2.Length();
        Vector3 u2 = len2 > float.Epsilon ? c2 / len2 : Vector3.UnitZ;

        // Make c1 orthogonal to u2: u1 = c1 - proj_u2(c1)
        Vector3 proj = Vector3.Dot(c1, u2) * u2;
        Vector3 u1 = c1 - proj;
        float len1 = u1.Length();
        if (len1 <= float.Epsilon)
        {
            // Choose an arbitrary vector not parallel to u2 to construct a valid orthonormal basis.
            Vector3 arbitrary = MathF.Abs(u2.X) < 0.9f ? Vector3.UnitX : Vector3.UnitY;
            u1 = Vector3.Normalize(Vector3.Cross(arbitrary, u2));
        }
        else
        {
            u1 /= len1;
        }

        // Compute u0 as cross(u1, u2) to preserve the original triad ordering.
        Vector3 u0 = Vector3.Cross(u1, u2);
        float len0 = u0.Length();
        if (len0 <= float.Epsilon)
        {
            // Fallback: pick any perpendicular vector
            u0 = Vector3.Normalize(Vector3.Cross(u2, u1));
        }
        else
        {
            u0 /= len0;
        }

        return mat
            .WithRow(0, new Vector4(u0, side.W))
            .WithRow(1, new Vector4(u1, roof.W))
            .WithRow(2, new Vector4(u2, nose.W));
    }

    /// <summary>
    /// Convert a vector into a vector of unit (1) length (normalize XYZ and preserve W).
    /// </summary>
    public static Vector4 UnitVector(Vector4 vec) => Vector4.Divide(vec, vec.Length());

    /// <summary>
    /// Calculate the dot product of two vectors sharing a common point.
    /// </summary>
    /// <returns>The cosine of the angle between the two vectors.</returns>
    public static float VectorDotProduct(Vector4 first, Vector4 second) => Vector4.Dot(first, second);

    private static void RotateVector(ref Vector4 vec, float alpha, float beta)
    {
        float x = vec.X;
        float y = vec.Y;
        float z = vec.Z;

        y -= alpha * x;
        x += alpha * y;
        y -= beta * z;
        z += beta * y;

        vec.X = x;
        vec.Y = y;
        vec.Z = z;
    }
}
