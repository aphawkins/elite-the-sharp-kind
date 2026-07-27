// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Assets.Models;

public sealed record FaceNormal
{
    public Vector4 Direction { get; set; }

    public bool Visible { get; set; }
}
