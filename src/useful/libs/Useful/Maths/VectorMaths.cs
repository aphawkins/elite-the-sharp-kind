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

    /// <summary>
    /// Tidy a 4x4 basis matrix: orthonormalise basis columns using Gram-Schmidt with safe fallbacks.
    /// Preserves translation/bottom-row components.
    /// </summary>
    public static Matrix4x4 OrthonormalizeBasis(Matrix4x4 mat)
    {
        // Extract basis columns consistent with existing MultiplyVector / ToVector3Array mapping:
        // column0 = (M11, M21, M31), column1 = (M12, M22, M32), column2 = (M13, M23, M33)
        Vector3 c1 = new(mat.M12, mat.M22, mat.M32);
        Vector3 c2 = new(mat.M13, mat.M23, mat.M33);

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

        // Rebuild matrix preserving the original translation and bottom row (M14..M44)
        return new Matrix4x4(
            u0.X,
            u1.X,
            u2.X,
            mat.M14,
            u0.Y,
            u1.Y,
            u2.Y,
            mat.M24,
            u0.Z,
            u1.Z,
            u2.Z,
            mat.M34,
            mat.M41,
            mat.M42,
            mat.M43,
            mat.M44);
    }

    /// <summary>
    /// Convert a vector into a vector of unit (1) length (normalize XYZ and preserve W).
    /// </summary>
    public static Vector4 UnitVector(Vector4 vec)
    {
        Vector3 v3 = new(vec.X, vec.Y, vec.Z);
        float len = v3.Length();
        if (len <= float.Epsilon)
        {
            return new Vector4(0f, 0f, 0f, vec.W);
        }

        Vector3 unit = v3 / len;
        return new Vector4(unit, vec.W);
    }

    /// <summary>
    /// Calculate the dot product of two vectors sharing a common point.
    /// </summary>
    /// <returns>The cosine of the angle between the two vectors.</returns>
    public static float VectorDotProduct(Vector4 first, Vector4 second) => Vector4.Dot(first, second);
}
