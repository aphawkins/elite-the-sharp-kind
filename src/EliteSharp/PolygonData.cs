// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp
{
    internal struct PolygonData
    {
        internal EColor FaceColour { get; set; }

        internal int Next { get; set; }

        internal Vector2[] PointList { get; set; }

        internal float Z { get; set; }
    }
}
