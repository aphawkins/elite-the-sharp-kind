// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp
{
    /// <summary>
    /// The original Elite code did all the vector calculations using 8-bit integers.
    /// </summary>
    internal static class VectorMaths
    {
        private static readonly Vector3[] s_startMatrix = new Vector3[3]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, -1),
        };

        internal static Vector3[] GetInitialMatrix() => s_startMatrix.Cloner();

        internal static Matrix4x4 RotateVector(Matrix4x4 mat, float alpha, float beta)
        {
            mat = mat.SetRow(0, RotateVector(mat.GetRow(0), alpha, beta));
            mat = mat.SetRow(1, RotateVector(mat.GetRow(1), alpha, beta));
            mat = mat.SetRow(2, RotateVector(mat.GetRow(2), alpha, beta));

            return mat;
        }

        internal static void TidyMatrix(Vector3[] mat)
        {
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
        internal static Vector3 UnitVector(Vector3 vec) => Vector3.Divide(vec, vec.Length());

        /// <summary>
        /// Calculate the dot product of two vectors sharing a common point.
        /// </summary>
        /// <returns>The cosine of the angle between the two vectors.</returns>
        internal static float VectorDotProduct(Vector3 first, Vector3 second) => Vector3.Dot(first, second);

        private static Vector3 RotateVector(Vector3 vec, float alpha, float beta)
        {
            Vector3 ret = vec;

            ret.Y -= alpha * ret.X;
            ret.X += alpha * ret.Y;
            ret.Y -= beta * ret.Z;
            ret.Z += beta * ret.Y;

            return ret;
        }
    }
}
