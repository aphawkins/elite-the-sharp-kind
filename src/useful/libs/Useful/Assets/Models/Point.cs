// 'Useful Libraries' - Andy Hawkins 2025.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Useful.Assets.Models;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public sealed record Point
{
    public required Collection<FaceNormal> FaceNormals { get; set; }

    public Vector4 Coords { get; set; }
}
