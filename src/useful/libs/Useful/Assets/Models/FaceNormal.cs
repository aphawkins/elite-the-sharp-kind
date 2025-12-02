// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Assets.Models;

public sealed record FaceNormal
{
    ////internal FaceNormal(int distance, Vector4 direction)
    ////{
    ////    Distance = distance;
    ////    Direction = direction;
    ////}

    public Vector4 Direction { get; set; }

    public int Distance { get; set; }

    public bool Visible { get; set; }
}
