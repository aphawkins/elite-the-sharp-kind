// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal class Python : ShipBase
    {
        internal Python(IDraw draw)
            : base(draw)
        {
            Type = ShipType.Python;
            Flags = ShipProperties.Trader;
            EnergyMax = 250;
            FaceNormals =
            [
                new(31, new(-27, 40, 11)),
                new(31, new(27, 40, 11)),
                new(31, new(-27, -40, 11)),
                new(31, new(27, -40, 11)),
                new(31, new(-19, 38, 0)),
                new(31, new(19, 38, 0)),
                new(31, new(-19, -38, 0)),
                new(31, new(19, -38, 0)),
                new(31, new(-25, 37, -11)),
                new(31, new(25, 37, -11)),
                new(31, new(25, -37, -11)),
                new(31, new(-25, -37, -11)),
                new(31, new(0, 0, -112)),
            ];
            Faces =
            [
                new(FastColors.DarkGrey, new(-0x1B, 0x28, 0x0B), [0, 1, 3]),
                new(FastColors.LightGrey, new(0x1B, 0x28, 0x0B), [2, 1, 0]),
                new(FastColors.LightGrey, new(-0x1B, -0x28, 0x0B), [0, 3, 8]),
                new(FastColors.DarkGrey, new(0x1B, -0x28, 0x0B), [8, 2, 0]),

                new(FastColors.DarkYellow, new(-0x13, 0x26, 0x00), [3, 1, 4]),
                new(FastColors.Gold, new(0x13, 0x26, 0x00), [4, 1, 2]),
                new(FastColors.Gold, new(-0x13, -0x26, 0x00), [3, 9, 8]),
                new(FastColors.DarkYellow, new(0x13, -0x26, 0x00), [8, 9, 2]),

                new(FastColors.DarkGrey, new(-0x19, 0x25, -0x0B), [3, 4, 5, 6]),
                new(FastColors.LightGrey, new(0x19, 0x25, -0x0B), [2, 7, 5, 4]),
                new(FastColors.DarkGrey, new(0x19, -0x25, -0x0B), [2, 9, 10, 7]),
                new(FastColors.LightGrey, new(-0x19, -0x25, -0x0B), [3, 6, 10, 9]),

                new(FastColors.DarkerGrey, new(0x00, 0x00, -0x70), [10, 6, 5, 7]),
            ];
            LaserStrength = 13;
            Lines =
            [
                new(31, 3, 2, 0, 8),
                new(31, 2, 0, 0, 3),
                new(31, 3, 1, 0, 2),
                new(31, 1, 0, 0, 1),
                new(31, 5, 9, 2, 4),
                new(31, 5, 1, 1, 2),
                new(31, 3, 7, 2, 8),
                new(31, 4, 0, 1, 3),
                new(31, 6, 2, 3, 8),
                new(31, 10, 7, 2, 9),
                new(31, 8, 4, 3, 4),
                new(31, 11, 6, 3, 9),
                new(7, 8, 8, 3, 5),
                new(7, 11, 11, 3, 10),
                new(7, 9, 9, 2, 5),
                new(7, 10, 10, 2, 10),
                new(31, 10, 9, 2, 7),
                new(31, 11, 8, 3, 6),
                new(31, 12, 8, 5, 6),
                new(31, 12, 9, 5, 7),
                new(31, 10, 12, 7, 10),
                new(31, 12, 11, 6, 10),
                new(31, 9, 8, 4, 5),
                new(31, 11, 10, 9, 10),
                new(31, 5, 4, 1, 4),
                new(31, 7, 6, 8, 9),
            ];
            LootMax = 5;
            MinDistance = 900;
            MissilesMax = 3;
            Name = "Python";
            Points =
            [
                new(new(0, 0, 224), 31, 1, 0, 3, 2),
                new(new(0, 48, 48), 31, 1, 0, 5, 4),
                new(new(96, 0, -16), 31, 15, 15, 15, 15),
                new(new(-96, 0, -16), 31, 15, 15, 15, 15),
                new(new(0, 48, -32), 31, 5, 4, 9, 8),
                new(new(0, 24, -112), 31, 8, 9, 12, 12),
                new(new(-48, 0, -112), 31, 11, 8, 12, 12),
                new(new(48, 0, -112), 31, 10, 9, 12, 12),
                new(new(0, -48, 48), 31, 3, 2, 7, 6),
                new(new(0, -48, -32), 31, 7, 6, 11, 10),
                new(new(0, -24, -112), 31, 11, 10, 12, 12),
            ];
            Size = 6400;
            VanishPoint = 40;
            VelocityMax = 20;
        }
    }
}
