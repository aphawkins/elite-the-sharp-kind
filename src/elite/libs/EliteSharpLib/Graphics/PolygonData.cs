// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Graphics;

internal struct PolygonData
{
    internal uint FaceColor { get; set; }

    internal int Next { get; set; }

    internal Vector2[] PointList { get; set; }

    // Depth per point, parallel to PointList, for the z-buffered fill.
    // ZBufferRenderer fills every entry with the face's flat Z key.
    // PainterRenderer leaves this field unused.
    internal float[] Depths { get; set; }

    internal float Z { get; set; }
}
