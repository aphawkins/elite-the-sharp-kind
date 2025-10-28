// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Maths;

public static class Extensions
{
    public static bool IsOdd(this int value) => value % 2 != 0;

    public static bool IsOdd(this float value) => ((int)value).IsOdd();

    public static Vector3 Cloner(this Vector3 vec) => new(vec.X, vec.Y, vec.Z);

    public static Vector3[] Cloner(this Vector3[] vecs)
    {
        Guard.ArgumentNull(vecs);

        Vector3[] newVecs = new Vector3[vecs.Length];
        for (int i = 0; i < vecs.Length; i++)
        {
            newVecs[i] = vecs[i].Cloner();
        }

        return newVecs;
    }

    internal static Vector2 ToVector2(this Vector3 vector) => new(vector.X, vector.Y);
}
