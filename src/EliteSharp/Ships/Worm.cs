// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class Worm : ShipBase
    {
        internal Worm(IDraw draw)
            : base(draw)
        {
            Type = ShipType.Worm;
            Flags = ShipProperties.PackHunter | ShipProperties.Slow | ShipProperties.Angry;
            EnergyMax = 30;
            FaceNormals =
            [
                new(31, new(0, 88, 70)),
                new(31, new(0, 69, 14)),
                new(31, new(70, 66, 35)),
                new(31, new(-70, 66, 35)),
                new(31, new(64, 49, 14)),
                new(31, new(-64, 49, 14)),
                new(31, new(0, 0, -200)),
                new(31, new(0, -80, 0)),
            ];
            Faces =
            [
                new(EliteColors.Grey, new(0x00, 0x58, 0x46), [1, 0, 2, 3]),
                new(EliteColors.LightGrey, new(0x46, 0x42, 0x23), [0, 4, 2]),
                new(EliteColors.LightGrey, new(-0x46, 0x42, 0x23), [1, 3, 5]),

                new(EliteColors.DarkGrey, new(0x40, 0x31, 0x0E), [2, 4, 6, 8]),
                new(EliteColors.DarkGrey, new(-0x40, 0x31, 0x0E), [5, 3, 9, 7]),
                new(EliteColors.LightGrey, new(0x00, 0x00, -0xC8), [6, 7, 9, 8]),

                new(EliteColors.DarkerGrey, new(0x00, -0x50, 0x00), [4, 0, 1, 5, 7, 6]),
                new(EliteColors.LightGrey, new(0x00, 0x45, 0x0E), [9, 3, 2, 8]),
            ];
            LaserStrength = 4;
            Lines =
            [
                new(31, 0, 7, 0, 1),
                new(31, 3, 7, 1, 5),
                new(31, 5, 7, 5, 7),
                new(31, 6, 7, 7, 6),
                new(31, 4, 7, 6, 4),
                new(31, 2, 7, 4, 0),
                new(31, 0, 2, 0, 2),
                new(31, 0, 3, 1, 3),
                new(31, 2, 4, 4, 2),
                new(31, 3, 5, 5, 3),
                new(31, 1, 4, 2, 8),
                new(31, 4, 6, 8, 6),
                new(31, 1, 5, 3, 9),
                new(31, 5, 6, 9, 7),
                new(31, 0, 1, 2, 3),
                new(31, 1, 6, 8, 9),
            ];
            MinDistance = 384;
            Name = "Worm";
            Points =
            [
                new(new(10, -10, 35), 31, 0, 2, 7, 7),
                new(new(-10, -10, 35), 31, 0, 3, 7, 7),
                new(new(5, 6, 15), 31, 0, 1, 2, 4),
                new(new(-5, 6, 15), 31, 0, 1, 3, 5),
                new(new(15, -10, 25), 31, 2, 4, 7, 7),
                new(new(-15, -10, 25), 31, 3, 5, 7, 7),
                new(new(26, -10, -25), 31, 4, 6, 7, 7),
                new(new(-26, -10, -25), 31, 5, 6, 7, 7),
                new(new(8, 14, -25), 31, 1, 4, 6, 6),
                new(new(-8, 14, -25), 31, 1, 5, 6, 6),
            ];
            Size = 9801;
            VanishPoint = 19;
            VelocityMax = 23;
        }
    }
}
