// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace Elite.Engine
{
    internal static class Extensions
    {
        public static bool IsOdd(this int value) => value % 2 != 0;

        public static bool IsOdd(this float value) => ((int)value).IsOdd();

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

#pragma warning disable CA1308 // Normalize strings to uppercase
        internal static string CapitaliseFirstLetter(this string text) => char.ToUpperInvariant(text[0]) + text[1..].ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
    }
}
