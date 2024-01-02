// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Trader;

namespace EliteSharp.Ships
{
    internal sealed class Tharglet : ShipBase
    {
        internal Tharglet(IDraw draw)
            : base(draw)
        {
            Type = ShipType.Tharglet;
            Flags = ShipProperties.Tharglet | ShipProperties.Angry;
            Bounty = 5;
            EnergyMax = 20;
            FaceNormals =
            [
                new(31, new(-36, 0, 0)),
                new(31, new(20, -5, 7)),
                new(31, new(46, -42, -14)),
                new(31, new(36, 0, -104)),
                new(31, new(46, 42, -14)),
                new(31, new(20, 5, 7)),
                new(31, new(36, 0, 0)),
            ];
            Faces =
            [
                new(EliteColors.LightRed, new(-0x24, 0x00, 0x00), [3, 2, 1, 0, 4]),

                new(EliteColors.LightGrey, new(0x14, -0x05, 0x07), [6, 5, 0, 1]),
                new(EliteColors.DarkGrey, new(0x2E, -0x2A, -0x0E), [7, 6, 1, 2]),
                new(EliteColors.Grey, new(0x24, 0x00, -0x68), [8, 7, 2, 3]),
                new(EliteColors.DarkGrey, new(0x2E, 0x2A, -0x0E), [9, 8, 3, 4]),
                new(EliteColors.DarkerGrey, new(0x14, 0x05, 0x07), [4, 0, 5, 9]),

                new(EliteColors.LightRed, new(0x24, 0x00, 0x00), [9, 5, 6, 7, 8]),
            ];
            LaserStrength = 8;
            Lines =
            [
                new(31, 1, 0, 0, 1),
                new(31, 2, 0, 1, 2),
                new(31, 3, 0, 2, 3),
                new(31, 4, 0, 3, 4),
                new(31, 5, 0, 0, 4),
                new(31, 5, 1, 0, 5),
                new(31, 2, 1, 1, 6),
                new(31, 3, 2, 2, 7),
                new(31, 4, 3, 3, 8),
                new(31, 5, 4, 4, 9),
                new(31, 6, 1, 5, 6),
                new(31, 6, 2, 6, 7),
                new(31, 6, 3, 7, 8),
                new(31, 6, 4, 8, 9),
                new(31, 6, 5, 9, 5),
            ];
            MinDistance = 384;
            Name = "Tharglet";
            Points =
            [
                new(new(-9, 0, 40), 31, 0, 1, 5, 5),
                new(new(-9, -38, 12), 31, 0, 1, 2, 2),
                new(new(-9, -24, -32), 31, 0, 2, 3, 3),
                new(new(-9, 24, -32), 31, 0, 3, 4, 4),
                new(new(-9, 38, 12), 31, 0, 4, 5, 5),
                new(new(9, 0, -8), 31, 1, 5, 6, 6),
                new(new(9, -10, -15), 31, 1, 2, 6, 6),
                new(new(9, -6, -26), 31, 2, 3, 6, 6),
                new(new(9, 6, -26), 31, 3, 4, 6, 6),
                new(new(9, 10, -15), 31, 4, 5, 6, 6),
            ];
            ScoopedType = StockType.AlienItems;
            Size = 1600;
            VanishPoint = 20;
            VelocityMax = 30;
        }
    }
}
