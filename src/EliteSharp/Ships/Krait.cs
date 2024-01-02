// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class Krait : ShipBase
    {
        internal Krait(IDraw draw)
            : base(draw)
        {
            Type = ShipType.Krait;
            Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
            Bounty = 10;
            EnergyMax = 80;
            FaceNormals =
            [
                new(31, new(3, 24, 3)),
                new(31, new(3, -24, 3)),
                new(31, new(-3, -24, 3)),
                new(31, new(-3, 24, 3)),
                new(31, new(38, 0, -77)),
                new(31, new(-38, 0, -77)),
            ];
            Faces =
            [
                new(EliteColors.DarkBlue, new(0x03, 0x18, 0x03), [0, 3, 1]),
                new(EliteColors.Blue, new(0x03, -0x18, 0x03), [2, 3, 0]),

                new(EliteColors.DarkBlue, new(-0x03, -0x18, 0x03), [0, 4, 2]),
                new(EliteColors.Blue, new(-0x03, 0x18, 0x03), [1, 4, 0]),

                new(EliteColors.DarkerGrey, new(0x26, 0x00, -0x4D), [3, 2, 1]),
                new(EliteColors.LightGrey, new(-0x26, 0x00, -0x4D), [4, 1, 2]),

                new(EliteColors.White, new(0x03, -0x18, 0x03), [3, 5]),
                new(EliteColors.White, new(0x03, 0x18, 0x03), [5, 3]),
                new(EliteColors.White, new(-0x03, 0x18, 0x03), [4, 6]),
                new(EliteColors.White, new(-0x03, -0x18, 0x03), [6, 4]),

                new(EliteColors.LighterRed, new(0x26, 0x00, -0x4D), [12, 11, 13]),
                new(EliteColors.LighterRed, new(-0x26, 0x00, -0x4D), [16, 14, 15]),
                new(EliteColors.White, new(0x03, 0x18, 0x03), [7, 10, 8]),
                new(EliteColors.White, new(-0x03, 0x18, 0x03), [8, 9, 7]),
            ];
            LaserStrength = 8;
            Lines =
            [
                new(31, 0, 3, 0, 1),
                new(31, 1, 2, 0, 2),
                new(31, 0, 1, 0, 3),
                new(31, 2, 3, 0, 4),
                new(31, 3, 5, 1, 4),
                new(31, 2, 5, 4, 2),
                new(31, 1, 4, 2, 3),
                new(31, 0, 4, 3, 1),
                new(30, 0, 1, 3, 5),
                new(30, 2, 3, 4, 6),
                new(8, 4, 5, 1, 2),
                new(9, 0, 0, 7, 10),
                new(6, 0, 0, 8, 10),
                new(9, 3, 3, 7, 9),
                new(6, 3, 3, 8, 9),
                new(8, 4, 4, 11, 13),
                new(8, 4, 4, 13, 12),
                new(7, 4, 4, 12, 11),
                new(7, 5, 5, 14, 15),
                new(8, 5, 5, 15, 16),
                new(8, 5, 5, 16, 14),
            ];
            LootMax = 1;
            MinDistance = 384;
            Name = "Krait";
            Points =
            [
                new(new(0, 0, 96), 31, 0, 1, 2, 3),
                new(new(0, 18, -48), 31, 0, 3, 4, 5),
                new(new(0, -18, -48), 31, 1, 2, 4, 5),
                new(new(90, 0, -3), 31, 0, 1, 4, 4),
                new(new(-90, 0, -3), 31, 2, 3, 5, 5),
                new(new(90, 0, 87), 30, 0, 1, 1, 1),
                new(new(-90, 0, 87), 30, 2, 3, 3, 3),
                new(new(0, 5, 53), 9, 0, 0, 3, 3),
                new(new(0, 7, 38), 6, 0, 0, 3, 3),
                new(new(-18, 7, 19), 9, 3, 3, 3, 3),
                new(new(18, 7, 19), 9, 0, 0, 0, 0),
                new(new(18, 11, -39), 8, 4, 4, 4, 4),
                new(new(18, -11, -39), 8, 4, 4, 4, 4),
                new(new(36, 0, -30), 8, 4, 4, 4, 4),
                new(new(-18, 11, -39), 8, 5, 5, 5, 5),
                new(new(-18, -11, -39), 8, 5, 5, 5, 5),
                new(new(-36, 0, -30), 8, 5, 5, 5, 5),
            ];
            Size = 3600;
            VanishPoint = 20;
            VelocityMax = 30;
        }
    }
}
