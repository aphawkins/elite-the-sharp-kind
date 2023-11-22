// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class DodecStation : ShipBase
    {
        internal DodecStation(IDraw draw)
            : base(draw)
        {
            Type = ShipType.Dodec;
            Flags = ShipProperties.Station;
            EnergyMax = 240;
            FaceNormals =
            [
                new(31, new(0, 0, 196)),
                new(31, new(103, 142, 88)),
                new(31, new(169, -55, 89)),
                new(31, new(0, -176, 88)),
                new(31, new(-169, -55, 89)),
                new(31, new(-103, 142, 88)),
                new(31, new(0, 176, -88)),
                new(31, new(169, 55, -89)),
                new(31, new(103, -142, -88)),
                new(31, new(-103, -142, -88)),
                new(31, new(-169, 55, -89)),
                new(31, new(0, 0, -196)),
            ];
            Faces =
            [
                new(FastColors.Grey, new(0x00, 0x00, 0xC4), [3, 2, 1, 0, 4]),
                new(FastColors.LightGrey, new(0x67, 0x8E, 0x58), [6, 10, 5, 0, 1]),
                new(FastColors.DarkGrey, new(0xA9, -0x37, 0x59), [7, 11, 6, 1, 2]),
                new(FastColors.DarkerGrey, new(0x00, -0xB0, 0x58), [8, 12, 7, 2, 3]),
                new(FastColors.LightGrey, new(-0xA9, -0x37, 0x59), [9, 13, 8, 3, 4]),
                new(FastColors.DarkerGrey, new(-0x67, 0x8E, 0x58), [5, 14, 9, 4, 0]),
                new(FastColors.LightGrey, new(0x00, 0xB0, -0x58), [15, 19, 14, 5, 10]),
                new(FastColors.DarkGrey, new(0xA9, 0x37, -0x59), [16, 15, 10, 6, 11]),
                new(FastColors.LightGrey, new(0x67, -0x8E, -0x58), [17, 16, 11, 7, 12]),
                new(FastColors.DarkerGrey, new(-0x67, -0x8E, -0x58), [18, 17, 12, 8, 13]),
                new(FastColors.DarkGrey, new(-0xA9, 0x37, -0x59), [19, 18, 13, 9, 14]),
                new(FastColors.Grey, new(0x00, 0x00, -0xC4), [19, 15, 16, 17, 18]),
                new(FastColors.Black, new(0x00, 0x00, 0xC4), [22, 20, 21, 23]),
            ];
            Lines =
            [
                new(31, 0, 1, 0, 1),
                new(31, 0, 2, 1, 2),
                new(31, 0, 3, 2, 3),
                new(31, 0, 4, 3, 4),
                new(31, 0, 5, 4, 0),
                new(31, 1, 6, 5, 10),
                new(31, 1, 7, 10, 6),
                new(31, 2, 7, 6, 11),
                new(31, 2, 8, 11, 7),
                new(31, 3, 8, 7, 12),
                new(31, 3, 9, 12, 8),
                new(31, 4, 9, 8, 13),
                new(31, 4, 10, 13, 9),
                new(31, 5, 10, 9, 14),
                new(31, 5, 6, 14, 5),
                new(31, 7, 11, 15, 16),
                new(31, 8, 11, 16, 17),
                new(31, 9, 11, 17, 18),
                new(31, 10, 11, 18, 19),
                new(31, 6, 11, 19, 15),
                new(31, 1, 5, 0, 5),
                new(31, 1, 2, 1, 6),
                new(31, 2, 3, 2, 7),
                new(31, 3, 4, 3, 8),
                new(31, 4, 5, 4, 9),
                new(31, 6, 7, 10, 15),
                new(31, 7, 8, 11, 16),
                new(31, 8, 9, 12, 17),
                new(31, 9, 10, 13, 18),
                new(31, 6, 10, 14, 19),
                new(30, 0, 0, 20, 21),
                new(20, 0, 0, 21, 23),
                new(23, 0, 0, 23, 22),
                new(20, 0, 0, 22, 20),
            ];
            MinDistance = 900;
            Name = "Dodec Space Station";
            Points =
            [
                new(new(0, 150, 196), 31, 0, 1, 5, 5),
                new(new(143, 46, 196), 31, 0, 1, 2, 2),
                new(new(88, -121, 196), 31, 0, 2, 3, 3),
                new(new(-88, -121, 196), 31, 0, 3, 4, 4),
                new(new(-143, 46, 196), 31, 0, 4, 5, 5),
                new(new(0, 243, 46), 31, 1, 5, 6, 6),
                new(new(231, 75, 46), 31, 1, 2, 7, 7),
                new(new(143, -196, 46), 31, 2, 3, 8, 8),
                new(new(-143, -196, 46), 31, 3, 4, 9, 9),
                new(new(-231, 75, 46), 31, 4, 5, 10, 10),
                new(new(143, 196, -46), 31, 1, 6, 7, 7),
                new(new(231, -75, -46), 31, 2, 7, 8, 8),
                new(new(0, -243, -46), 31, 3, 8, 9, 9),
                new(new(-231, -75, -46), 31, 4, 9, 10, 10),
                new(new(-143, 196, -46), 31, 5, 6, 10, 10),
                new(new(88, 121, -196), 31, 6, 7, 11, 11),
                new(new(143, -46, -196), 31, 7, 8, 11, 11),
                new(new(0, -150, -196), 31, 8, 9, 11, 11),
                new(new(-143, -46, -196), 31, 9, 10, 11, 11),
                new(new(-88, 121, -196), 31, 6, 10, 11, 11),
                new(new(-16, 32, 196), 30, 0, 0, 0, 0),
                new(new(-16, -32, 196), 30, 0, 0, 0, 0),
                new(new(16, 32, 196), 23, 0, 0, 0, 0),
                new(new(16, -32, 196), 23, 0, 0, 0, 0),
            ];
            Size = 32400;
            VanishPoint = 125;
        }
    }
}
