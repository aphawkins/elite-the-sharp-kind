// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.Engine
{
    internal struct PolygonData
    {
        internal Colour FaceColour { get; set; }

        internal int Next { get; set; }

        internal Vector2[] PointList { get; set; }

        internal float Z { get; set; }
    }
}
