// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships;

internal sealed class Shuttle : ShipBase
{
    internal Shuttle(IDraw draw)
        : base(draw)
    {
        Type = ShipType.Shuttle;
        Flags = ShipProperties.SpaceJunk | ShipProperties.FlyToPlanet | ShipProperties.Slow;
        EnergyMax = 32;
        FaceNormals =
        [
            new(31, new(-55, -55, 40)),
            new(31, new(0, -74, 4)),
            new(31, new(-51, -51, 23)),
            new(31, new(-74, 0, 4)),
            new(31, new(-51, 51, 23)),
            new(31, new(0, 74, 4)),
            new(31, new(51, 51, 23)),
            new(31, new(74, 0, 4)),
            new(31, new(51, -51, 23)),
            new(31, new(0, 0, -107)),
            new(31, new(-41, 41, 90)),
            new(31, new(41, 41, 90)),
            new(31, new(55, -55, 40)),
        ];
        Faces =
        [
            new(EliteColors.LightGrey, new(0x00, -0x4A, 0x04), [0, 4, 7]),
            new(EliteColors.DarkGrey, new(-0x33, -0x33, 0x17), [1, 4, 0]),
            new(EliteColors.LightGrey, new(-0x4A, 0x00, 0x04), [1, 5, 4]),
            new(EliteColors.DarkGrey, new(-0x33, 0x33, 0x17), [2, 5, 1]),
            new(EliteColors.LightGrey, new(0x00, 0x4A, 0x04), [2, 6, 5]),
            new(EliteColors.DarkGrey, new(0x33, 0x33, 0x17), [3, 6, 2]),
            new(EliteColors.LightGrey, new(0x4A, 0x00, 0x04), [3, 7, 6]),
            new(EliteColors.DarkGrey, new(0x33, -0x33, 0x17), [0, 7, 3]),

            new(EliteColors.DarkerGrey, new(0x00, 0x00, -0x6B), [7, 4, 5, 6]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0x6B), [11, 8, 9, 10]),

            new(EliteColors.Grey, new(-0x37, -0x37, 0x28), [0, 12, 1]),
            new(EliteColors.LightGrey, new(-0x29, 0x29, 0x5A), [1, 12, 2]),
            new(EliteColors.Grey, new(0x29, 0x29, 0x5A), [2, 12, 3]),
            new(EliteColors.LightGrey, new(0x37, -0x37, 0x28), [3, 12, 0]),

            new(EliteColors.LightBlue, new(0x29, 0x29, 0x5A), [14, 13, 15]),
            new(EliteColors.LightBlue, new(-0x29, 0x29, 0x5A), [18, 16, 17]),
        ];
        Lines =
        [
            new(31, 0, 2, 0, 1),
            new(31, 4, 10, 1, 2),
            new(31, 6, 11, 2, 3),
            new(31, 8, 12, 0, 3),
            new(31, 1, 8, 0, 7),
            new(24, 1, 2, 0, 4),
            new(31, 2, 3, 1, 4),
            new(24, 3, 4, 1, 5),
            new(31, 4, 5, 2, 5),
            new(12, 5, 6, 2, 6),
            new(31, 6, 7, 3, 6),
            new(24, 7, 8, 3, 7),
            new(31, 3, 9, 4, 5),
            new(31, 5, 9, 5, 6),
            new(31, 7, 9, 6, 7),
            new(31, 1, 9, 4, 7),
            new(16, 0, 12, 0, 12),
            new(16, 0, 10, 1, 12),
            new(16, 10, 11, 2, 12),
            new(16, 11, 12, 3, 12),
            new(16, 9, 9, 8, 9),
            new(7, 9, 9, 9, 10),
            new(9, 9, 9, 10, 11),
            new(7, 9, 9, 8, 11),
            new(5, 11, 11, 13, 14),
            new(8, 11, 11, 14, 15),
            new(7, 11, 11, 13, 15),
            new(5, 10, 10, 16, 17),
            new(8, 10, 10, 17, 18),
            new(7, 10, 10, 16, 18),
        ];
        LootMax = 15;
        MinDistance = 200;
        Name = "Orbit Shuttle";
        Points =
        [
            new(new(0, -17, 23), 31, 15, 15, 15, 15),
            new(new(-17, 0, 23), 31, 15, 15, 15, 15),
            new(new(0, 18, 23), 31, 15, 15, 15, 15),
            new(new(18, 0, 23), 31, 15, 15, 15, 15),
            new(new(-20, -20, -27), 31, 1, 2, 3, 9),
            new(new(-20, 20, -27), 31, 3, 4, 5, 9),
            new(new(20, 20, -27), 31, 5, 6, 7, 9),
            new(new(20, -20, -27), 31, 1, 7, 8, 9),
            new(new(5, 0, -27), 16, 9, 9, 9, 9),
            new(new(0, -2, -27), 16, 9, 9, 9, 9),
            new(new(-5, 0, -27), 9, 9, 9, 9, 9),
            new(new(0, 3, -27), 9, 9, 9, 9, 9),
            new(new(0, -9, 35), 16, 0, 10, 11, 12),
            new(new(3, -1, 31), 7, 15, 15, 0, 2),
            new(new(4, 11, 25), 8, 0, 1, 15, 4),
            new(new(11, 4, 25), 8, 10, 1, 3, 15),
            new(new(-3, -1, 31), 7, 6, 11, 2, 3),
            new(new(-3, 11, 25), 8, 15, 8, 12, 0),
            new(new(-10, 4, 25), 8, 4, 15, 1, 8),
        ];
        Size = 2500;
        VanishPoint = 22;
        VelocityMax = 8;
    }
}
