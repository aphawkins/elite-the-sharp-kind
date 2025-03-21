// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships;

internal sealed class RockHermit : ShipBase
{
    internal RockHermit(IDraw draw)
        : base(draw)
    {
        Type = ShipType.Hermit;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Slow;
        EnergyMax = 180;
        FaceNormals =
        [
            new(31, new(9, 66, 81)),
            new(31, new(9, -66, 81)),
            new(31, new(-72, 64, 31)),
            new(31, new(-64, -73, 47)),
            new(31, new(45, -79, 65)),
            new(31, new(135, 15, 35)),
            new(31, new(38, 76, 70)),
            new(31, new(-66, 59, -39)),
            new(31, new(-67, -15, -80)),
            new(31, new(66, -14, -75)),
            new(31, new(-70, -80, -40)),
            new(31, new(58, -102, -51)),
            new(31, new(81, 9, -67)),
            new(31, new(47, 94, -63)),
        ];
        Faces =
        [
            new(EliteColors.Lilac, new(0x09, 0x42, 0x51), [5, 0, 6]),
            new(EliteColors.LightGrey, new(0x09, -0x42, 0x51), [2, 5, 6]),
            new(EliteColors.DarkGrey, new(-0x48, 0x40, 0x1F), [6, 0, 1]),
            new(EliteColors.Lilac, new(-0x40, -0x49, 0x2F), [2, 6, 1]),
            new(EliteColors.DarkGrey, new(0x2D, -0x4F, 0x41), [3, 5, 2]),
            new(EliteColors.LightGrey, new(0x87, 0x0F, 0x23), [4, 5, 3]),
            new(EliteColors.DarkGrey, new(0x26, 0x4C, 0x46), [0, 5, 4]),
            new(EliteColors.Lilac, new(-0x42, 0x3B, -0x27), [1, 0, 7]),
            new(EliteColors.LightGrey, new(-0x43, -0x0F, -0x50), [1, 7, 8]),
            new(EliteColors.DarkGrey, new(0x42, -0x0E, -0x4B), [3, 8, 7]),
            new(EliteColors.DarkGrey, new(-0x46, -0x50, -0x28), [1, 8, 2]),
            new(EliteColors.Lilac, new(0x3A, -0x66, -0x33), [3, 2, 8]),
            new(EliteColors.Lilac, new(0x51, 0x09, -0x43), [4, 3, 7]),
            new(EliteColors.LightGrey, new(0x2F, 0x5E, -0x3F), [4, 7, 0]),
        ];
        LaserStrength = 1;
        Lines =
        [
            new(31, 7, 2, 0, 1),
            new(31, 13, 6, 0, 4),
            new(31, 12, 5, 3, 4),
            new(31, 11, 4, 2, 3),
            new(31, 10, 3, 1, 2),
            new(31, 3, 2, 1, 6),
            new(31, 3, 1, 2, 6),
            new(31, 4, 1, 2, 5),
            new(31, 1, 0, 5, 6),
            new(31, 6, 0, 0, 5),
            new(31, 5, 4, 3, 5),
            new(31, 2, 0, 0, 6),
            new(31, 6, 5, 4, 5),
            new(31, 10, 8, 1, 8),
            new(31, 8, 7, 1, 7),
            new(31, 13, 7, 0, 7),
            new(31, 13, 12, 4, 7),
            new(31, 12, 9, 3, 7),
            new(31, 11, 9, 3, 8),
            new(31, 11, 10, 2, 8),
            new(31, 9, 8, 7, 8),
        ];
        LootMax = 7;
        MinDistance = 384;
        MissilesMax = 2;
        Name = "Rock Hermit";
        Points =
        [
            new(new(0, 80, 0), 31, 15, 15, 15, 15),
            new(new(-80, -10, 0), 31, 15, 15, 15, 15),
            new(new(0, -80, 0), 31, 15, 15, 15, 15),
            new(new(70, -40, 0), 31, 15, 15, 15, 15),
            new(new(60, 50, 0), 31, 6, 5, 13, 12),
            new(new(50, 0, 60), 31, 15, 15, 15, 15),
            new(new(-40, 0, 70), 31, 1, 0, 3, 2),
            new(new(0, 30, -75), 31, 15, 15, 15, 15),
            new(new(0, -50, -60), 31, 9, 8, 11, 10),
        ];
        Size = 6400;
        VanishPoint = 50;
        VelocityMax = 30;
    }
}
