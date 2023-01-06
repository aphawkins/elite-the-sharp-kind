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
	using System;
	using System.Numerics;
	using Elite.Structs;

	internal static class VectorMaths
	{
		static Vector3[] start_matrix = new Vector3[3]
		{
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(0.0f, 1.0f, 0.0f),
			new Vector3(0.0f, 0.0f,-1.0f)
		};

		/*
		 * Multiply first matrix by second matrix.
		 * Put result into first matrix.
		 */
		static void mult_matrix(Vector3[] first, Vector3[] second)
		{
			int i;
			Vector3[] rv = new Vector3[3];
			float x;
			float y;
			float z;

			for (i = 0; i < 3; i++)
			{
				x = (first[0].X * second[i].X) + (first[1].X * second[i].Y) + (first[2].X * second[i].Z);
				y = (first[0].Y * second[i].X) + (first[1].Y * second[i].Y) + (first[2].Y * second[i].Z);
				z = (first[0].Z * second[i].X) + (first[1].Z * second[i].Y) + (first[2].Z * second[i].Z);

				rv[i] = new(x, y, z);
			}

			for (i = 0; i < 3; i++)
			{
				first[i] = rv[i];
			}
		}

		internal static void mult_vector(ref Vector3 vec, Vector3[] mat)
		{
			float x;
			float y;
			float z;

			x = (vec.X * mat[0].X) +
				(vec.Y * mat[0].Y) +
				(vec.Z * mat[0].Z);

			y = (vec.X * mat[1].X) +
				(vec.Y * mat[1].Y) +
				(vec.Z * mat[1].Z);

			z = (vec.X * mat[2].X) +
				(vec.Y * mat[2].Y) +
				(vec.Z * mat[2].Z);

			vec.X = x;
			vec.Y = y;
			vec.Z = z;
		}

		/*
		 * Calculate the dot product of two vectors sharing a common point.
		 * Returns the cosine of the angle between the two vectors.
		 */
		internal static float vector_dot_product(Vector3 first, Vector3 second)
		{
			return (first.X * second.X) + (first.Y * second.Y) + (first.Z * second.Z);
		}

		/*
		 * Convert a vector into a vector of unit (1) length.
		 */
		internal static Vector3 unit_vector(Vector3 vec)
		{
			Vector3 res;

			float lx = vec.X;
			float ly = vec.Y;
			float lz = vec.Z;

			float uni = MathF.Sqrt(lx * lx + ly * ly + lz * lz);

			res.X = lx / uni;
			res.Y = ly / uni;
			res.Z = lz / uni;

			return res;
		}

		internal static void set_init_matrix(ref Vector3[] mat)
		{
			int i;

			for (i = 0; i < 3; i++)
			{
				mat[i] = start_matrix[i];
			}
		}

		internal static void tidy_matrix(Vector3[] mat)
		{
			mat[2] = unit_vector(mat[2]);

			if ((mat[2].X > -1) && (mat[2].X < 1))
			{
				if ((mat[2].Y > -1) && (mat[2].Y < 1))
				{
					mat[1].Z = -(mat[2].X * mat[1].X + mat[2].Y * mat[1].Y) / mat[2].Z;
				}
				else
				{
					mat[1].Y = -(mat[2].X * mat[1].X + mat[2].Z * mat[1].Z) / mat[2].Y;
				}
			}
			else
			{
				mat[1].X = -(mat[2].Y * mat[1].Y + mat[2].Z * mat[1].Z) / mat[2].X;
			}

			mat[1] = unit_vector(mat[1]);

			/* xyzzy... nothing happens. :-)*/

			mat[0].X = mat[1].Y * mat[2].Z - mat[1].Z * mat[2].Y;
			mat[0].Y = mat[1].Z * mat[2].X - mat[1].X * mat[2].Z;
			mat[0].Z = mat[1].X * mat[2].Y - mat[1].Y * mat[2].X;
		}
	}
}