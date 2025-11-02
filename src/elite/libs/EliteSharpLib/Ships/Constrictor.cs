// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Constrictor : ShipBase
{
    internal Constrictor(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Constrictor;
        Flags = ShipProperties.LoneWolf | ShipProperties.Angry;
        EnergyMax = 252;
        FaceNormals =
        [
            new(31, new(0, 55, 15)),
            new(31, new(-24, 75, 20)),
            new(31, new(24, 75, 20)),
            new(31, new(44, 75, 0)),
            new(31, new(-44, 75, 0)),
            new(31, new(-44, 75, 0)),
            new(31, new(0, 53, 0)),
            new(31, new(44, 75, 0)),
            new(31, new(0, 0, -160)),
            new(31, new(0, -27, 0)),
        ];
        Faces =
        [
            new(EliteColors.Grey, new(0x00, 0x37, 0x0F), [1, 0, 8, 9]),
            new(EliteColors.LightGrey, new(-0x18, 0x4B, 0x14), [1, 9, 2]),
            new(EliteColors.LightGrey, new(0x18, 0x4B, 0x14), [0, 7, 8]),

            new(EliteColors.Gold, new(0x2C, 0x4B, 0x00), [7, 6, 8]),
            new(EliteColors.Gold, new(-0x2C, 0x4B, 0x00), [9, 3, 2]),

            new(EliteColors.DarkYellow, new(-0x2C, 0x4B, 0x00), [9, 4, 3]),
            new(EliteColors.LightGrey, new(0x00, 0x35, 0x00), [8, 5, 4, 9]),
            new(EliteColors.DarkYellow, new(0x2C, 0x4B, 0x00), [8, 6, 5]),

            new(EliteColors.DarkGrey, new(0x00, 0x00, -0xA0), [6, 3, 4, 5]),
            new(EliteColors.DarkerGrey, new(0x00, -0x1B, 0x00), [3, 6, 7, 0, 1, 2]),
            new(EliteColors.LightRed, new(0x00, -0x1B, 0x00), [12, 10, 14]),
            new(EliteColors.LightRed, new(0x00, -0x1B, 0x00), [15, 11, 13]),
        ];
        LaserStrength = 26;
        Lines =
        [
            new(31, 0, 9, 0, 1),
            new(31, 1, 9, 1, 2),
            new(31, 0, 1, 1, 9),
            new(31, 0, 2, 0, 8),
            new(31, 2, 9, 0, 7),
            new(31, 2, 3, 7, 8),
            new(31, 1, 4, 2, 9),
            new(31, 4, 9, 2, 3),
            new(31, 3, 9, 6, 7),
            new(31, 3, 7, 6, 8),
            new(31, 6, 7, 5, 8),
            new(31, 5, 6, 4, 9),
            new(31, 4, 5, 3, 9),
            new(31, 5, 8, 3, 4),
            new(31, 6, 8, 4, 5),
            new(31, 7, 8, 5, 6),
            new(31, 8, 9, 3, 6),
            new(31, 0, 6, 8, 9),
            new(18, 9, 9, 10, 12),
            new(5, 9, 9, 12, 14),
            new(10, 9, 9, 14, 10),
            new(10, 9, 9, 11, 15),
            new(5, 9, 9, 13, 15),
            new(18, 9, 9, 11, 13),
        ];
        LootMax = 3;
        MissilesMax = 4;
        Name = "Constrictor";
        Points =
        [
            new(new(20, -7, 80), 31, 0, 2, 9, 9),
            new(new(-20, -7, 80), 31, 0, 1, 9, 9),
            new(new(-54, -7, 40), 31, 1, 4, 9, 9),
            new(new(-54, -7, -40), 31, 4, 5, 8, 9),
            new(new(-20, 13, -40), 31, 5, 6, 8, 8),
            new(new(20, 13, -40), 31, 6, 7, 8, 8),
            new(new(54, -7, -40), 31, 3, 7, 8, 9),
            new(new(54, -7, 40), 31, 2, 3, 9, 9),
            new(new(20, 13, 5), 31, 15, 15, 15, 15),
            new(new(-20, 13, 5), 31, 15, 15, 15, 15),
            new(new(20, -7, 62), 18, 9, 9, 9, 9),
            new(new(-20, -7, 62), 18, 9, 9, 9, 9),
            new(new(25, -7, -25), 18, 9, 9, 9, 9),
            new(new(-25, -7, -25), 18, 9, 9, 9, 9),
            new(new(15, -7, -15), 10, 9, 9, 9, 9),
            new(new(-15, -7, -15), 10, 9, 9, 9, 9),
            new(new(0, -7, 0), 0, 9, 15, 0, 1),
        ];
        Size = 4225;
        VanishPoint = 45;
        VelocityMax = 36;
    }
}
