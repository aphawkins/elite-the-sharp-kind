namespace Elite.Engine
{
    using System.Numerics;
    using Elite.Engine.Enums;

    internal struct PolygonData
    {
        internal float Z;
        internal GFX_COL face_colour;
        internal Vector2[] point_list;
        internal int next;
    };
}