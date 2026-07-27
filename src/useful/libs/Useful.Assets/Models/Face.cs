// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets.Models;

public sealed record Face
{
    public FastColor Color { get; set; }

    public required IList<Point> Points { get; init; }

    public required IList<int> PointIndices { get; init; }
}
