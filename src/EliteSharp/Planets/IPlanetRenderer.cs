// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Planets
{
    internal interface IPlanetRenderer
    {
        void Draw(Vector2 centre, float radius, Vector3[] vec);
    }
}
