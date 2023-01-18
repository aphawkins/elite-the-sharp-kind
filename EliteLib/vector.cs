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


/*
 * The original Elite code did all the vector calculations using 8-bit integers.
 *
 * Writing all the routines in C to use 8 bit ints would have been fairly pointless.
 * I have, therefore, written a new set of routines which use floating point math.
 */

namespace Elite
{
    using System.Numerics;

    internal static class VectorMaths
	{
		static Vector3[] start_matrix = new Vector3[3]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0f,-1f)
		};

		internal static void mult_vector(ref Vector3 vec, Vector3[] mat)
		{
			Matrix4x4 matrix = new(
				mat[0].X, mat[0].Y, mat[0].Z, 0f,
				mat[1].X, mat[1].Y, mat[1].Z, 0f,
				mat[2].X, mat[2].Y, mat[2].Z, 0f,
				0f, 0f, 0f, 0f);

            vec = Vector3.TransformNormal(vec, matrix);
		}

		/// <summary>
		/// Calculate the dot product of two vectors sharing a common point.
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns>The cosine of the angle between the two vectors.</returns>
		internal static float vector_dot_product(Vector3 first, Vector3 second)
		{
            return Vector3.Dot(first, second);
        }

		/// <summary>
		/// Convert a vector into a vector of unit (1) length.
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		internal static Vector3 unit_vector(Vector3 vec)
		{
            return Vector3.Divide(vec, vec.Length());
		}

		internal static void set_init_matrix(ref Vector3[] mat)
		{
			for (int i = 0; i < 3; i++)
			{
				mat[i] = start_matrix[i];
			}
		}

		internal static void tidy_matrix(Vector3[] mat)
		{
			mat[2] = unit_vector(mat[2]);

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

			mat[1] = unit_vector(mat[1]);

			/* xyzzy... nothing happens. :-)*/

			mat[0].X = (mat[1].Y * mat[2].Z) - (mat[1].Z * mat[2].Y);
			mat[0].Y = (mat[1].Z * mat[2].X) - (mat[1].X * mat[2].Z);
			mat[0].Z = (mat[1].X * mat[2].Y) - (mat[1].Y * mat[2].X);
		}

        internal static void rotate_vec(ref Vector3[] vec, float alpha, float beta)
		{
			for (int i = 0; i < vec.Length; i++)
			{
				rotate_vec(ref vec[i], alpha, beta);
            }
        }

        static void rotate_vec(ref Vector3 vec, float alpha, float beta)
        {
            float x = vec.X;
            float y = vec.Y;
            float z = vec.Z;

            y -= (alpha * x);
            x += (alpha * y);
            y -= (beta * z);
            z += (beta * y);

            vec.X = x;
            vec.Y = y;
            vec.Z = z;
        }
    }
}