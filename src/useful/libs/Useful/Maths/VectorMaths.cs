// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Maths;

public static class VectorMaths
{
    private static readonly Vector3[] s_startMatrix =
    [
        new(1, 0, 0),
        new(0, 1, 0),
        new(0, 0, -1),
    ];

    public static Vector3[] GetInitialMatrix() => s_startMatrix.Cloner();

    public static Vector3 MultiplyVector(Vector3 vec, Vector3[] mat)
    {
        Guard.ArgumentNull(mat);

        Matrix4x4 matrix = new(
            mat[0].X,
            mat[1].X,
            mat[2].X,
            0,
            mat[0].Y,
            mat[1].Y,
            mat[2].Y,
            0,
            mat[0].Z,
            mat[1].Z,
            mat[2].Z,
            0,
            0,
            0,
            0,
            0);

        return Vector3.Transform(vec, matrix);
    }

    public static Vector3[] RotateVector(Vector3[] vec, float alpha, float beta)
    {
        Guard.ArgumentNull(vec);

        for (int i = 0; i < vec.Length; i++)
        {
            RotateVector(ref vec[i], alpha, beta);
        }

        return vec;
    }

    public static void TidyMatrix(Vector3[] mat)
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
    public static Vector3 UnitVector(Vector3 vec) => Vector3.Divide(vec, vec.Length());

    /// <summary>
    /// Calculate the dot product of two vectors sharing a common point.
    /// </summary>
    /// <returns>The cosine of the angle between the two vectors.</returns>
    public static float VectorDotProduct(Vector3 first, Vector3 second) => Vector3.Dot(first, second);

    private static void RotateVector(ref Vector3 vec, float alpha, float beta)
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
