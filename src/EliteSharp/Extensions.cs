// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Ships;

namespace EliteSharp
{
    internal static class Extensions
    {
        internal static bool IsOdd(this int value) => value % 2 != 0;

        internal static bool IsOdd(this float value) => ((int)value).IsOdd();

        internal static Vector3 Cloner(this Vector3 vec) => new(vec.X, vec.Y, vec.Z);

        internal static Vector3[] Cloner(this Vector3[] vecs)
        {
            Vector3[] newVecs = new Vector3[vecs.Length];
            for (int i = 0; i < vecs.Length; i++)
            {
                newVecs[i] = vecs[i].Cloner();
            }

            return newVecs;
        }

        internal static Vector2 ToVector2(this Vector3 vector) => new(vector.X, vector.Y);

        internal static Matrix4x4 ToMatrix(this Vector3[] mat) => new(
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

        internal static Vector3[] ToVectors(this Matrix4x4 mat)
        {
            Vector3[] vecs = new Vector3[3];

            vecs[0].X = mat[0, 0];
            vecs[1].X = mat[0, 1];
            vecs[2].X = mat[0, 2];

            vecs[0].Y = mat[1, 0];
            vecs[1].Y = mat[1, 1];
            vecs[2].Y = mat[1, 2];

            vecs[0].Z = mat[2, 0];
            vecs[1].Z = mat[2, 1];
            vecs[2].Z = mat[2, 2];

            return vecs;
        }

        internal static Vector3 GetRow(this Matrix4x4 mat, int row) => new(mat[0, row], mat[1, row], mat[2, row]);

        internal static Matrix4x4 SetRow(this Matrix4x4 mat, int row, Vector3 vec)
        {
            Matrix4x4 ret = mat;
            ret[0, row] = vec.X;
            ret[1, row] = vec.Y;
            ret[2, row] = vec.Z;
            return ret;
        }

#pragma warning disable CA1308 // Normalize strings to uppercase
        internal static string CapitaliseFirstLetter(this string text) => char.ToUpperInvariant(text[0]) + text[1..].ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

        internal static void CopyTo(this IShip from, IShip to)
        {
            from.CopyTo((IObject)to);

            to.Bounty = from.Bounty;
            to.EnergyMax = from.EnergyMax;
            to.FaceNormals = from.FaceNormals;
            to.Faces = from.Faces;
            to.LaserFront = from.LaserFront;
            to.LaserStrength = from.LaserStrength;
            to.Lines = from.Lines;
            to.LootMax = from.LootMax;
            to.MissilesMax = from.MissilesMax;
            to.Name = from.Name;
            to.Points = from.Points;
            to.ScoopedType = from.ScoopedType;
            to.Size = from.Size;
            to.VanishPoint = from.VanishPoint;
            to.VelocityMax = from.VelocityMax;
            to.ExpDelta = from.ExpDelta;
            to.Flags = from.Flags;
            to.Type = from.Type;
            to.Location = from.Location.Cloner();
            to.Energy = from.Energy;
            to.Velocity = from.Velocity;
            to.Acceleration = from.Acceleration;
            to.Missiles = from.Missiles;
            to.Target = from.Target;
            to.Bravery = from.Bravery;
            to.MinDistance = from.MinDistance;
        }

        internal static void CopyTo(this IObject from, IObject to)
        {
            to.Flags = from.Flags;
            to.Rotmat = from.Rotmat.Cloner();
            to.RotX = from.RotX;
            to.RotZ = from.RotZ;
            to.Type = from.Type;
            to.Location = from.Location.Cloner();
        }
    }
}
