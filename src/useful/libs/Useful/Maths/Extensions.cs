// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Maths;

public static class Extensions
{
    public static bool IsOdd(this int value) => value % 2 != 0;

    public static bool IsOdd(this float value) => ((int)value).IsOdd();

    public static Vector4 Cloner(this Vector4 vec) => new(vec.X, vec.Y, vec.Z, vec.W);

    public static Vector4[] Cloner(this Vector4[] vecs)
    {
        Guard.ArgumentNull(vecs);

        Vector4[] newVecs = new Vector4[vecs.Length];
        for (int i = 0; i < vecs.Length; i++)
        {
            newVecs[i] = vecs[i].Cloner();
        }

        return newVecs;
    }

    public static Matrix4x4 ToMatrix4x4(this Vector4[] vecs)
    {
        Guard.ArgumentNull(vecs);

        if (vecs.Length < 4)
        {
            throw new ArgumentException("Vector array must contain at least four vectors.", nameof(vecs));
        }

        Vector4 v0 = vecs[0];
        Vector4 v1 = vecs[1];
        Vector4 v2 = vecs[2];
        Vector4 v3 = vecs[3];

        // Keep the same layout used elsewhere in the codebase (matches MultiplyVector construction).
        return new Matrix4x4(
            v0.X,
            v1.X,
            v2.X,
            v3.X,
            v0.Y,
            v1.Y,
            v2.Y,
            v2.Y,
            v0.Z,
            v1.Z,
            v2.Z,
            v3.Z,
            v0.W,
            v1.W,
            v2.W,
            v3.W);
    }

    public static Vector4[] ToVector4Array(this Matrix4x4 matrix)
        => [
            new Vector4(matrix.M11, matrix.M21, matrix.M31, matrix.M41),
            new Vector4(matrix.M12, matrix.M22, matrix.M32, matrix.M42),
            new Vector4(matrix.M13, matrix.M23, matrix.M33, matrix.M43),
            new Vector4(matrix.M14, matrix.M24, matrix.M34, matrix.M44),
        ];

    internal static Vector2 ToVector2(this Vector4 vector) => new(vector.X, vector.Y);
}
