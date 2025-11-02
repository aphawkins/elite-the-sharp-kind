// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Boa : ShipBase
{
    internal Boa(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Boa;
        Flags = ShipProperties.Trader;
        EnergyMax = 250;
        FaceNormals =
        [
            new(31, new(43, 37, -60)),
            new(31, new(0, -45, -89)),
            new(31, new(-43, 37, -60)),
            new(31, new(0, 40, 0)),
            new(31, new(62, -32, -20)),
            new(31, new(-62, -32, -20)),
            new(31, new(0, 23, 6)),
            new(31, new(-23, -15, 9)),
            new(31, new(23, -15, 9)),
            new(31, new(-26, 13, 10)),
            new(31, new(0, -31, 12)),
            new(31, new(26, 13, 10)),
            new(14, new(0, 0, -107)),
        ];
        Faces =
        [
            new(EliteColors.Purple, new(0x2B, 0x25, -0x3C), [11, 10, 1, 5, 6, 2]),
            new(EliteColors.Blue, new(0x00, -0x2D, -0x59), [12, 11, 2, 7, 8, 3]),
            new(EliteColors.DarkBlue, new(-0x2B, 0x25, -0x3C), [3, 9, 4, 1, 10, 12]),

            new(EliteColors.Purple, new(0x00, 0x28, 0x00), [5, 1, 4]),
            new(EliteColors.Blue, new(0x3E, -0x20, -0x14), [7, 2, 6]),
            new(EliteColors.DarkBlue, new(-0x3E, -0x20, -0x14), [3, 8, 9]),

            new(EliteColors.LightGrey, new(0x00, 0x17, 0x06), [5, 4, 0]),
            new(EliteColors.LightGrey, new(-0x17, -0x0F, 0x09), [9, 8, 0]),
            new(EliteColors.LightGrey, new(0x17, -0x0F, 0x09), [7, 6, 0]),
            new(EliteColors.DarkGrey, new(-0x1A, 0x0D, 0x0A), [0, 4, 9]),
            new(EliteColors.DarkGrey, new(0x00, -0x1F, 0x0C), [0, 8, 7]),
            new(EliteColors.DarkGrey, new(0x1A, 0x0D, 0x0A), [0, 6, 5]),

            new(EliteColors.LightRed, new(0x00, 0x00, -0x6B), [12, 10, 11]),
        ];
        LaserStrength = 14;
        Lines =
        [
            new(31, 6, 11, 0, 5),
            new(31, 8, 10, 0, 7),
            new(31, 7, 9, 0, 9),
            new(29, 6, 9, 0, 4),
            new(29, 8, 11, 0, 6),
            new(29, 7, 10, 0, 8),
            new(31, 3, 6, 4, 5),
            new(31, 0, 11, 5, 6),
            new(31, 4, 8, 6, 7),
            new(31, 1, 10, 7, 8),
            new(31, 5, 7, 8, 9),
            new(31, 2, 9, 4, 9),
            new(24, 2, 3, 1, 4),
            new(24, 0, 3, 1, 5),
            new(24, 2, 5, 3, 9),
            new(24, 1, 5, 3, 8),
            new(24, 0, 4, 2, 6),
            new(24, 1, 4, 2, 7),
            new(22, 0, 2, 1, 10),
            new(22, 0, 1, 2, 11),
            new(22, 1, 2, 3, 12),
            new(14, 0, 12, 10, 11),
            new(14, 1, 12, 11, 12),
            new(14, 2, 12, 12, 10),
        ];
        LootMax = 5;
        MinDistance = 500;
        MissilesMax = 4;
        Name = "Boa";
        Points =
        [
            new(new(0, 0, 93), 31, 15, 15, 15, 15),
            new(new(0, 40, -87), 24, 0, 2, 3, 3),
            new(new(38, -25, -99), 24, 0, 1, 4, 4),
            new(new(-38, -25, -99), 24, 1, 2, 5, 5),
            new(new(-38, 40, -59), 31, 2, 3, 6, 9),
            new(new(38, 40, -59), 31, 0, 3, 6, 11),
            new(new(62, 0, -67), 31, 0, 4, 8, 11),
            new(new(24, -65, -79), 31, 1, 4, 8, 10),
            new(new(-24, -65, -79), 31, 1, 5, 7, 10),
            new(new(-62, 0, -67), 31, 2, 5, 7, 9),
            new(new(0, 7, -107), 22, 0, 2, 10, 10),
            new(new(13, -9, -107), 22, 0, 1, 10, 10),
            new(new(-13, -9, -107), 22, 1, 2, 12, 12),
        ];
        Size = 4900;
        VanishPoint = 40;
        VelocityMax = 24;
    }
}
