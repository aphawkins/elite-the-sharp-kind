// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets.Models;

public sealed record Face
{
    public FastColor Color { get; set; }

    public required IList<Point> Points { get; init; }

    public required IList<int> PointIndices { get; init; }
}
