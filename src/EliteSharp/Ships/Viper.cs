// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships;

internal sealed class Viper : ShipBase
{
    internal Viper(IDraw draw)
        : base(draw)
    {
        Type = ShipType.Viper;
        Flags = ShipProperties.Police | ShipProperties.Bold;
        EnergyMax = 140;
        FaceNormals =
        [
            new(31, new(0, 32, 0)),
            new(31, new(-22, 33, 11)),
            new(31, new(22, 33, 11)),
            new(31, new(-22, -33, 11)),
            new(31, new(22, -33, 11)),
            new(31, new(0, -32, 0)),
            new(31, new(0, 0, -48)),
        ];
        Faces =
        [
            new(EliteColors.DarkGrey, new(0x00, 0x20, 0x00), [7, 8, 1]),

            new(EliteColors.DarkBlue, new(-0x16, 0x21, 0x0B), [8, 4, 0, 1]),
            new(EliteColors.Blue, new(0x16, 0x21, 0x0B), [3, 7, 1, 0]),

            new(EliteColors.Blue, new(-0x16, -0x21, 0x0B), [2, 0, 4, 6]),
            new(EliteColors.DarkBlue, new(0x16, -0x21, 0x0B), [0, 2, 5, 3]),

            new(EliteColors.DarkGrey, new(0x00, -0x20, 0x00), [2, 6, 5]),
            new(EliteColors.LightGrey, new(0x00, 0x00, -0x30), [4, 8, 7, 3, 5, 6]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0x30), [12, 13, 9]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0x30), [10, 14, 11]),
        ];
        LaserStrength = 8;
        Lines =
        [
            new(31, 4, 2, 0, 3),
            new(30, 2, 1, 0, 1),
            new(30, 4, 3, 0, 2),
            new(31, 3, 1, 0, 4),
            new(30, 2, 0, 1, 7),
            new(30, 1, 0, 1, 8),
            new(30, 5, 4, 2, 5),
            new(30, 5, 3, 2, 6),
            new(31, 6, 0, 7, 8),
            new(30, 6, 5, 5, 6),
            new(31, 6, 1, 4, 8),
            new(30, 6, 3, 4, 6),
            new(31, 6, 2, 3, 7),
            new(30, 4, 6, 3, 5),
            new(19, 6, 6, 9, 12),
            new(18, 6, 6, 9, 13),
            new(19, 6, 6, 10, 11),
            new(18, 6, 6, 10, 14),
            new(16, 6, 6, 11, 14),
            new(16, 6, 6, 12, 13),
        ];
        MinDistance = 384;
        MissilesMax = 1;
        Name = "Viper";
        Points =
        [
            new(new(0, 0, 72), 31, 2, 1, 4, 3),
            new(new(0, 16, 24), 30, 1, 0, 2, 2),
            new(new(0, -16, 24), 30, 4, 3, 5, 5),
            new(new(48, 0, -24), 31, 4, 2, 6, 6),
            new(new(-48, 0, -24), 31, 3, 1, 6, 6),
            new(new(24, -16, -24), 30, 5, 4, 6, 6),
            new(new(-24, -16, -24), 30, 3, 5, 6, 6),
            new(new(24, 16, -24), 31, 2, 0, 6, 6),
            new(new(-24, 16, -24), 31, 1, 0, 6, 6),
            new(new(-32, 0, -24), 19, 6, 6, 6, 6),
            new(new(32, 0, -24), 19, 6, 6, 6, 6),
            new(new(8, 8, -24), 19, 6, 6, 6, 6),
            new(new(-8, 8, -24), 19, 6, 6, 6, 6),
            new(new(-8, -8, -24), 18, 6, 6, 6, 6),
            new(new(8, -8, -24), 18, 6, 6, 6, 6),
        ];
        Size = 5625;
        VanishPoint = 23;
        VelocityMax = 32;
    }
}
