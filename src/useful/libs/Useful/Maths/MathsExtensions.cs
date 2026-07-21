// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Maths;

public static class MathsExtensions
{
    public static bool IsOdd(this int value) => value % 2 != 0;

    public static bool IsOdd(this float value) => ((int)value).IsOdd();

    public static Vector4 Cloner(this Vector4 vec) => new(vec.X, vec.Y, vec.Z, vec.W);

    /// <summary>
    /// Read row <paramref name="row"/> (0-3) of the matrix as a <see cref="Vector4"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="row"/> is not 0-3.</exception>
    public static Vector4 GetRow(this Matrix4x4 m, int row) => row switch
    {
        0 => new Vector4(m.M11, m.M12, m.M13, m.M14),
        1 => new Vector4(m.M21, m.M22, m.M23, m.M24),
        2 => new Vector4(m.M31, m.M32, m.M33, m.M34),
        3 => new Vector4(m.M41, m.M42, m.M43, m.M44),
        _ => throw new ArgumentOutOfRangeException(nameof(row)),
    };

    /// <summary>
    /// Return a copy of the matrix with row <paramref name="row"/> (0-3) replaced by <paramref name="value"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="row"/> is not 0-3.</exception>
    public static Matrix4x4 WithRow(this Matrix4x4 m, int row, Vector4 value)
    {
        switch (row)
        {
            case 0:
                m.M11 = value.X;
                m.M12 = value.Y;
                m.M13 = value.Z;
                m.M14 = value.W;
                break;

            case 1:
                m.M21 = value.X;
                m.M22 = value.Y;
                m.M23 = value.Z;
                m.M24 = value.W;
                break;

            case 2:
                m.M31 = value.X;
                m.M32 = value.Y;
                m.M33 = value.Z;
                m.M34 = value.W;
                break;

            case 3:
                m.M41 = value.X;
                m.M42 = value.Y;
                m.M43 = value.Z;
                m.M44 = value.W;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(row));
        }

        return m;
    }

    internal static Vector2 ToVector2(this Vector4 vector) => new(vector.X, vector.Y);
}
