// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Trader;

namespace EliteSharp.Ships;

internal sealed class RockSplinter : ShipBase
{
    internal RockSplinter(IDraw draw)
        : base(draw)
    {
        Type = ShipType.Rock;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        EnergyMax = 20;
        FaceNormals =
        [
            new(18, new(30, 0, 0)),
            new(20, new(22, 32, -8)),
            new(0, new(0, 2, 0)),
            new(0, new(17, 23, 95)),
        ];
        Faces =
        [
            new(EliteColors.LightGrey, new(0x00, 0x00, 0x00), [3, 2, 1]),
            new(EliteColors.DarkGrey, new(0x00, 0x00, 0x00), [0, 2, 3]),
            new(EliteColors.DarkerGrey, new(0x00, 0x00, 0x00), [3, 1, 0]),
            new(EliteColors.Grey, new(0x00, 0x00, 0x00), [0, 1, 2]),
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
        Name = "Rock Splinter";
        Points =
        [
            new(new(-24, -25, 16), 31, 1, 2, 3, 3),
            new(new(0, 12, -10), 31, 0, 2, 3, 3),
            new(new(11, -6, 2), 31, 0, 1, 3, 3),
            new(new(12, 42, 7), 31, 0, 1, 2, 2),
        ];
        ScoopedType = StockType.Minerals;
        Size = 256;
        VanishPoint = 8;
        VelocityMax = 10;
    }
}
