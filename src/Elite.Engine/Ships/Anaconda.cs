// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Enums;

namespace EliteSharp.Ships
{
    internal sealed class Anaconda : NullObject
    {
        internal Anaconda()
        {
            Type = ShipType.Anaconda;
            Flags = ShipFlags.Slow;
            EnergyMax = 252;
            FaceNormals = new ShipFaceNormal[]
            {
                new(30, new(0,  -51,  -49)),
                new(30, new(-51,   18,  -87)),
                new(30, new(-77,  -57,  -19)),
                new(31, new(0,  -90,   16)),
                new(30, new(77,  -57,  -19)),
                new(30, new(51,   18,  -87)),
                new(30, new(0,  111,  -20)),
                new(31, new(-97,   72,   24)),
                new(31, new(-108,  -68,   34)),
                new(31, new(108,  -68,   34)),
                new(31, new(97,   72,   24)),
                new(31, new(0,   94,   18)),
            };
            Faces = new ShipFace[]
            {
                new(Colour.Green, new(0x00, -0x33, -0x31), new[] { 3,  2,  1,  0, 4 }),
                new(Colour.LightGreen, new(-0x33, 0x12, -0x57), new[] { 6, 10,  5,  0, 1 }),
                new(Colour.LighterGreen, new(-0x4D, -0x39, -0x13), new[] { 7, 11,  6,  1, 2 }),

                new(Colour.DarkGrey, new(0x00, -0x5A, 0x10), new[] { 8, 12,  7,  2, 3 }),

                new(Colour.LightGreen, new(0x4D, -0x39, -0x13), new[] { 9, 13,  8,  3, 4 }),
                new(Colour.LighterGreen, new(0x33, 0x12, -0x57), new[] { 9,  4,  0,  5, 14 }),
                new(Colour.Green, new(0x00, 0x6F, -0x14), new[] { 10, 14, 5 }),

                new(Colour.DarkGrey, new(-0x61, 0x48, 0x18), new[] { 10, 6,  11, 12 }),
                new(Colour.LightGrey, new(-0x6C, -0x44, 0x22), new[] { 7, 12, 11 }),
                new(Colour.LightGrey, new(0x6C, -0x44, 0x22), new[] { 8, 13, 12 }),
                new(Colour.DarkGrey, new(0x61, 0x48, 0x18), new[] { 9, 14,  12, 13 }),
                new(Colour.LightGrey, new(0x00, 0x5E, 0x12), new[] { 10, 12, 14 }),
            };
            LaserFront = 12;
            LaserStrength = 31;
            Lines = new ShipLine[]
            {
                new(30,  0,  1,  0,  1),
                new(30,  0,  2,  1,  2),
                new(30,  0,  3,  2,  3),
                new(30,  0,  4,  3,  4),
                new(30,  0,  5,  0,  4),
                new(29,  1,  5,  0,  5),
                new(29,  1,  2,  1,  6),
                new(29,  2,  3,  2,  7),
                new(29,  3,  4,  3,  8),
                new(29,  4,  5,  4,  9),
                new(30,  1,  6,  5, 10),
                new(30,  1,  7,  6, 10),
                new(30,  2,  7,  6, 11),
                new(30,  2,  8,  7, 11),
                new(31,  3,  8,  7, 12),
                new(31,  3,  9,  8, 12),
                new(30,  4,  9,  8, 13),
                new(30,  4, 10,  9, 13),
                new(30,  5, 10,  9, 14),
                new(30,  5,  6,  5, 14),
                new(30,  6, 11, 10, 14),
                new(31,  7, 11, 10, 12),
                new(31,  7,  8, 11, 12),
                new(31,  9, 10, 12, 13),
                new(31, 10, 11, 12, 14),
            };
            LootMax = 7;
            MissilesMax = 7;
            Name = "Anaconda";
            Points = new ShipPoint[]
            {
                new(new(0,    7,  -58), 30,  0,  1,  5,  5),
                new(new(-43,  -13,  -37), 30,  0,  1,  2,  2),
                new(new(-26,  -47,   -3), 30,  0,  2,  3,  3),
                new(new(26,  -47,   -3), 30,  0,  3,  4,  4),
                new(new(43,  -13,  -37), 30,  0,  4,  5,  5),
                new(new(0,   48,  -49), 30,  1,  5,  6,  6),
                new(new(-69,   15,  -15), 30,  1,  2,  7,  7),
                new(new(-43,  -39,   40), 31,  2,  3,  8,  8),
                new(new(43,  -39,   40), 31,  3,  4,  9,  9),
                new(new(69,   15,  -15), 30,  4,  5, 10, 10),
                new(new(-43,   53,  -23), 31, 15, 15, 15, 15),
                new(new(-69,   -1,   32), 31,  2,  7,  8,  8),
                new(new(0,    0,  254), 31, 15, 15, 15, 15),
                new(new(69,   -1,   32), 31,  4,  9, 10, 10),
                new(new(43,   53,  -23), 31, 15, 15, 15, 15),
            };
            Size = 10000;
            Class = ShipClass.Trader;
            VanishPoint = 36;
            VelocityMax = 14;
        }
    }
}
