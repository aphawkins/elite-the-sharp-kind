// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Graphics.Rendering;

internal struct PolygonData
{
    internal FastColor Color { get; set; }

    internal int Next { get; set; }

    internal Vector2[] PointList { get; set; }

    // Camera-space depth per point, parallel to PointList, for the
    // z-buffered fill. PainterRenderer leaves this field unused, as
    // ZBufferRenderer leaves Next and Z unused.
    internal float[] Depths { get; set; }

    internal float Z { get; set; }
}
