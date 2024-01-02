// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class Coriolis : ShipBase
    {
        internal Coriolis(IDraw draw)
            : base(draw)
        {
            Type = ShipType.Coriolis;
            Flags = ShipProperties.Station;
            EnergyMax = 240;
            FaceNormals =
            [
                new(31, new(0, 0, 160)),
                new(31, new(107, -107, 107)),
                new(31, new(107, 107, 107)),
                new(31, new(-107, 107, 107)),
                new(31, new(-107, -107, 107)),
                new(31, new(0, -160, 0)),
                new(31, new(160, 0, 0)),
                new(31, new(-160, 0, 0)),
                new(31, new(0, 160, 0)),
                new(31, new(-107, -107, -107)),
                new(31, new(107, -107, -107)),
                new(31, new(107, 107, -107)),
                new(31, new(-107, 107, -107)),
                new(31, new(0, 0, -160)),
            ];
            Faces =
            [
                new(EliteColors.DarkerGrey, new(0x6B, -0x6B, 0x6B), [4, 0, 3]),
                new(EliteColors.DarkerGrey, new(0x6B, 0x6B, 0x6B), [0, 5, 1]),
                new(EliteColors.DarkerGrey, new(-0x6B, 0x6B, 0x6B), [1, 6, 2]),
                new(EliteColors.DarkerGrey, new(-0x6B, -0x6B, 0x6B), [2, 7, 3]),

                new(EliteColors.DarkGrey, new(0x00, -0xA0, 0x00), [4, 3, 7, 11]),
                new(EliteColors.DarkGrey, new(0xA0, 0x00, 0x00), [8, 5, 0, 4]),
                new(EliteColors.DarkGrey, new(-0xA0, 0x00, 0x00), [10, 7, 2, 6]),
                new(EliteColors.DarkGrey, new(0x00, 0xA0, 0x00), [1, 5, 9, 6]),

                new(EliteColors.DarkerGrey, new(-0x6B, -0x6B, -0x6B), [11, 7, 10]),
                new(EliteColors.DarkerGrey, new(0x6B, -0x6B, -0x6B), [11, 8, 4]),
                new(EliteColors.DarkerGrey, new(0x6B, 0x6B, -0x6B), [9, 5, 8]),
                new(EliteColors.DarkerGrey, new(-0x6B, 0x6B, -0x6B), [10, 6, 9]),

                new(EliteColors.LightGrey, new(0x00, 0x00, -0xA0), [11, 10, 9, 8]),
                new(EliteColors.LightGrey, new(0x00, 0x00, 0xA0), [0, 1, 2, 3]),

                new(EliteColors.Black, new(0x00, 0x00, 0xA0), [15, 12, 13, 14]),
            ];
            LaserStrength = 3;
            Lines =
            [
                new(31, 1, 0, 0, 3),
                new(31, 2, 0, 0, 1),
                new(31, 3, 0, 1, 2),
                new(31, 4, 0, 2, 3),
                new(31, 5, 1, 3, 4),
                new(31, 6, 1, 0, 4),
                new(31, 6, 2, 0, 5),
                new(31, 8, 2, 5, 1),
                new(31, 8, 3, 1, 6),
                new(31, 7, 3, 2, 6),
                new(31, 7, 4, 2, 7),
                new(31, 5, 4, 3, 7),
                new(31, 13, 10, 8, 11),
                new(31, 13, 11, 8, 9),
                new(31, 13, 12, 9, 10),
                new(31, 13, 9, 10, 11),
                new(31, 10, 5, 4, 11),
                new(31, 10, 6, 4, 8),
                new(31, 11, 6, 5, 8),
                new(31, 11, 8, 5, 9),
                new(31, 12, 8, 6, 9),
                new(31, 12, 7, 6, 10),
                new(31, 9, 7, 7, 10),
                new(31, 9, 5, 7, 11),
                new(30, 0, 0, 12, 13),
                new(30, 0, 0, 13, 14),
                new(30, 0, 0, 14, 15),
                new(30, 0, 0, 15, 12),
            ];
            MinDistance = 800;
            MissilesMax = 6;
            Name = "Coriolis Space Station";
            Points =
            [
                new(new(160, 0, 160), 31, 1, 0, 6, 2),
                new(new(0, 160, 160), 31, 2, 0, 8, 3),
                new(new(-160, 0, 160), 31, 3, 0, 7, 4),
                new(new(0, -160, 160), 31, 1, 0, 5, 4),
                new(new(160, -160, 0), 31, 5, 1, 10, 6),
                new(new(160, 160, 0), 31, 6, 2, 11, 8),
                new(new(-160, 160, 0), 31, 7, 3, 12, 8),
                new(new(-160, -160, 0), 31, 5, 4, 9, 7),
                new(new(160, 0, -160), 31, 10, 6, 13, 11),
                new(new(0, 160, -160), 31, 11, 8, 13, 12),
                new(new(-160, 0, -160), 31, 9, 7, 13, 12),
                new(new(0, -160, -160), 31, 9, 5, 13, 10),
                new(new(10, -30, 160), 30, 0, 0, 0, 0),
                new(new(10, 30, 160), 30, 0, 0, 0, 0),
                new(new(-10, 30, 160), 30, 0, 0, 0, 0),
                new(new(-10, -30, 160), 30, 0, 0, 0, 0),
            ];
            Size = 25600;
            VanishPoint = 120;
        }
    }
}
