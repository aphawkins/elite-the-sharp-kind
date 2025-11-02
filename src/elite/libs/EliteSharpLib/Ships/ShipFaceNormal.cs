// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Ships;

internal sealed class ShipFaceNormal
{
    internal ShipFaceNormal(int distance, Vector3 direction)
    {
        Distance = distance;
        Direction = new(direction, 0);
    }

    internal Vector4 Direction { get; set; }

    internal int Distance { get; set; }
}
