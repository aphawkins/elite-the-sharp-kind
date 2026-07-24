// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Assets.Models;

public record ThreeDModel
{
    public required IList<FaceNormal> FaceNormals { get; init; }

    public required IList<Face> Faces { get; init; }

    public required IList<Line> Lines { get; init; }

    public required IList<Point> Points { get; init; }
}
