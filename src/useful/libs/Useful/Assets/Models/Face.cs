// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Useful.Assets.Models;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public sealed record Face
{
    ////internal Face(uint color, Vector4 normal, Point[] points)
    ////{
    ////    Color = color;
    ////    Normal = normal;
    ////    Points = points;
    ////}

    public uint Color { get; set; }

    public Vector4 Normal { get; set; }

    public required IList<Point> Points { get; set; }
}
