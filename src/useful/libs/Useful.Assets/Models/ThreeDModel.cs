// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics.CodeAnalysis;

namespace Useful.Assets.Models;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public record ThreeDModel
{
    public required IList<FaceNormal> FaceNormals { get; set; }

    public required IList<Face> Faces { get; set; }

    public required IList<Line> Lines { get; set; }

    public required IList<Point> Points { get; set; }
}
