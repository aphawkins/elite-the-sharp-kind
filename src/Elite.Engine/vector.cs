/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 */


using System.Numerics;

/*
 * The original Elite code did all the vector calculations using 8-bit integers.
 *
 * Writing all the routines in C to use 8 bit ints would have been fairly pointless.
 * I have, therefore, written a new set of routines which use floating point math.
 */

namespace Elite.Engine
{
    internal static class VectorMaths
    {
        private static readonly Vector3[] start_matrix = new Vector3[3]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0,-1)
        };

        internal static Vector3 MultiplyVector(Vector3 vec, Vector3[] mat)
        {
            Matrix4x4 matrix = new(
                mat[0].X, mat[1].X, mat[2].X, 0,
                mat[0].Y, mat[1].Y, mat[2].Y, 0,
                mat[0].Z, mat[1].Z, mat[2].Z, 0,
                0, 0, 0, 0);

            return Vector3.Transform(vec, matrix);
        }

        /// <summary>
        /// Calculate the dot product of two vectors sharing a common point.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>The cosine of the angle between the two vectors.</returns>
        internal static float VectorDotProduct(Vector3 first, Vector3 second) => Vector3.Dot(first, second);

        /// <summary>
        /// Convert a vector into a vector of unit (1) length.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        internal static Vector3 UnitVector(Vector3 vec) => Vector3.Divide(vec, vec.Length());

        internal static Vector3[] GetInitialMatrix() => start_matrix.Cloner();

        internal static void TidyMatrix(Vector3[] mat)
        {
            mat[2] = UnitVector(mat[2]);

            if (mat[2].X is > (-1) and < 1)
            {
                if (mat[2].Y is > (-1) and < 1)
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

            /* xyzzy... nothing happens. :-)*/

            mat[0].X = (mat[1].Y * mat[2].Z) - (mat[1].Z * mat[2].Y);
            mat[0].Y = (mat[1].Z * mat[2].X) - (mat[1].X * mat[2].Z);
            mat[0].Z = (mat[1].X * mat[2].Y) - (mat[1].Y * mat[2].X);
        }

        internal static void RotateVector(ref Vector3[] vec, float alpha, float beta)
        {
            for (int i = 0; i < vec.Length; i++)
            {
                RotateVector(ref vec[i], alpha, beta);
            }
        }

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
}