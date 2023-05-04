using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.Engine
{
    internal struct PolygonData
    {
        internal float Z;
        internal GFX_COL face_colour;
        internal Vector2[] point_list;
        internal int next;
    };
}