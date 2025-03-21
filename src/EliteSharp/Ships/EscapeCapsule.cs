// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Trader;

namespace EliteSharp.Ships;

internal sealed class EscapeCapsule : ShipBase
{
    internal EscapeCapsule(IDraw draw)
        : base(draw)
    {
        Type = ShipType.EscapeCapsule;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Slow | ShipProperties.FlyToPlanet;
        EnergyMax = 17;
        FaceNormals =
        [
            new(31, new(52, 0, -122)),
            new(31, new(39, 103, 30)),
            new(31, new(39, -103, 30)),
            new(31, new(-112, 0, 0)),
        ];
        Faces =
        [
            new(EliteColors.LighterRed, new(0x34, 0x00, -0x7A), [3, 1, 2]),
            new(EliteColors.LightRed, new(0x27, 0x67, 0x1E), [0, 3, 2]),
            new(EliteColors.Red, new(0x27, -0x67, 0x1E), [0, 1, 3]),
            new(EliteColors.RedOrange, new(0x70, 0x00, 0x00), [0, 2, 1]),
        ];
        Lines =
        [
            new(31, 2, 3, 0, 1),
            new(31, 0, 3, 1, 2),
            new(31, 0, 1, 2, 3),
            new(31, 1, 2, 3, 0),
            new(31, 1, 3, 0, 2),
            new(31, 0, 2, 3, 1),
        ];
        MinDistance = 200;
        Name = "Escape Capsule";
        Points =
        [
            new(new(-7, 0, 36), 31, 1, 2, 3, 3),
            new(new(-7, -14, -12), 31, 0, 2, 3, 3),
            new(new(-7, 14, -12), 31, 0, 1, 3, 3),
            new(new(21, 0, 0), 31, 0, 1, 2, 2),
        ];
        ScoopedType = StockType.Slaves;
        Size = 256;
        VanishPoint = 8;
        VelocityMax = 8;
    }
}
