// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Collections.ObjectModel;
using System.Numerics;

namespace Useful.Assets.Models;

public sealed record Point
{
    public required Collection<FaceNormal> FaceNormals { get; init; }

    public Vector4 Coords { get; set; }
}
