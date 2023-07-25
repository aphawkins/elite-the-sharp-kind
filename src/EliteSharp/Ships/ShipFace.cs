// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class ShipFace
    {
        internal ShipFace(EColor colour, Vector3 normal, int[] points)
        {
            Colour = colour;
            Normal = normal;
            Points = points;
        }

        internal EColor Colour { get; set; }

        internal Vector3 Normal { get; set; }

        internal int[] Points { get; set; }
    }
}
