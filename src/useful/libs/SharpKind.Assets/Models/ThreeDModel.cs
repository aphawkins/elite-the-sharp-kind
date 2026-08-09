// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets.Models;

public record ThreeDModel
{
    public required IList<FaceNormal> FaceNormals { get; init; }

    public required IList<Face> Faces { get; init; }

    public required IList<Line> Lines { get; init; }

    public required IList<Point> Points { get; init; }
}
