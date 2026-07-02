// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics.CodeAnalysis;

namespace Useful.Assets.Models;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public sealed record Face
{
    public uint Color { get; set; }

    public required IList<Point> Points { get; set; }
}
