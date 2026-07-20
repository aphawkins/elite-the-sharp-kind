// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics.Rendering;

internal struct PolygonData
{
    internal uint Color { get; set; }

    internal int Next { get; set; }

    internal Vector2[] PointList { get; set; }

    // Depth per point, parallel to PointList, for the z-buffered fill.
    // ZBufferRenderer fills every entry with the polygon's flat Z key.
    // PainterRenderer leaves this field unused.
    internal float[] Depths { get; set; }

    internal float Z { get; set; }
}
