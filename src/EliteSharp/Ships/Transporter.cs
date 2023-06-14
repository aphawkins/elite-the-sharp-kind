// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class Transporter : NullObject
    {
        internal Transporter()
        {
            Type = ShipType.Transporter;
            Flags = ShipFlags.FlyToPlanet | ShipFlags.Slow;
            EnergyMax = 32;
            FaceNormals = new ShipFaceNormal[]
            {
                new(31, new(0, 0, -103)),
                new(31, new(-111, 48, -7)),
                new(31, new(-105, -63, -21)),
                new(31, new(0, -34, 0)),
                new(31, new(105, -63, -21)),
                new(31, new(111, 48, -7)),
                new(31, new(8, 32, 3)),
                new(31, new(-8, 32, 3)),
                new(19, new(-8, 34, 11)),
                new(31, new(-75, 32, 79)),
                new(31, new(75, 32, 79)),
                new(19, new(8, 34, 11)),
                new(31, new(0, 38, 17)),
                new(31, new(0, 0, 121)),
            };
            Faces = new ShipFace[]
            {
                new(Colour.DarkerGrey, new(0x00, 0x00, -0x67), new[] { 5,  4,  3, 2,  1, 0, 6 }),

                new(Colour.LightBlue, new(-0x6F, 0x30, -0x07), new[] { 9,  8,  1,  2 }),
                new(Colour.Blue, new(-0x69, -0x3F, -0x15), new[] { 3,  9,  2 }),
                new(Colour.Purple, new(0x00, -0x22, 0x00), new[] { 14, 13,  9,  3, 4, 10 }),
                new(Colour.Blue, new(0x69, -0x3F, -0x15), new[] { 5, 10,  4 }),
                new(Colour.LightBlue, new(0x6F, 0x30, -0x07), new[] { 11, 10,  5,  6 }),

                new(Colour.LightGrey, new(0x08, 0x20, 0x03), new[] { 6,  0,  7, 11 }),
                new(Colour.DarkGrey, new(-0x08, 0x20, 0x03), new[] { 8,  7,  0,  1 }),

                new(Colour.LightBlue, new(-0x4B, 0x20, 0x4F), new[] { 13, 12,  8,  9 }),
                new(Colour.LightBlue, new(0x4B, 0x20, 0x4F), new[] { 15, 14, 10, 11 }),

                new(Colour.LightGrey, new(-0x08, 0x22, 0x0B), new[] { 8, 12,  7 }),
                new(Colour.DarkGrey, new(0x08, 0x22, 0x0B), new[] { 7, 15, 11 }),
                new(Colour.Grey, new(0x00, 0x26, 0x11), new[] { 7, 12,  15 }),

                new(Colour.LighterGrey, new(0x00, 0x00, 0x79), new[] { 15, 12, 13, 14 }),
                new(Colour.LightRed, new(0x00, 0x00, -0x67), new[] { 35, 34, 33, 36 }),

                new(Colour.White, new(0x00, -0x22, 0x00), new[] { 30, 29 /*, 31 */ }),
                new(Colour.White, new(0x00, -0x22, 0x00), new[] { 31, 32 /*, 29 */ }),

                new(Colour.White, new(-0x08, 0x20, 0x03), new[] { 17, 16 /*, 18 */ }),
                new(Colour.White, new(-0x08, 0x20, 0x03), new[] { 18, 19 /*, 16 */ }),
                new(Colour.White, new(-0x08, 0x20, 0x03), new[] { 18, 20 /*, 19 */ }),
                new(Colour.White, new(-0x08, 0x20, 0x03), new[] { 20, 21 /*, 18 */ }),
                new(Colour.White, new(-0x08, 0x20, 0x03), new[] { 20, 19 /*, 21 */ }),

                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 23, 22 /*, 26 */ }),
                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 25, 26 /*, 23 */ }),
                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 24, 22 /*, 25 */ }),
                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 24, 23 /*, 22 */ }),
                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 28, 27 /*, 23 */ }),
                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 25, 27 /*, 22 */ }),
                new(Colour.White, new(0x08, 0x20, 0x03), new[] { 27, 26 /*, 22 */ }),
            };
            LaserFront = 12;
            Lines = new ShipLine[]
            {
                new(31,  0,  7,  0,  1),
                new(31,  0,  1,  1,  2),
                new(31,  0,  2,  2,  3),
                new(31,  0,  3,  3,  4),
                new(31,  0,  4,  4,  5),
                new(31,  0,  5,  5,  6),
                new(31,  0,  6,  0,  6),
                new(16,  6,  7,  0,  7),
                new(31,  1,  7,  1,  8),
                new(11,  1,  2,  2,  9),
                new(31,  2,  3,  3,  9),
                new(31,  3,  4,  4, 10),
                new(11,  4,  5,  5, 10),
                new(31,  5,  6,  6, 11),
                new(17,  7,  8,  7,  8),
                new(17,  1,  9,  8,  9),
                new(17,  5, 10, 10, 11),
                new(17,  6, 11,  7, 11),
                new(19, 11, 12,  7, 15),
                new(19,  8, 12,  7, 12),
                new(16,  8,  9,  8, 12),
                new(31,  3,  9,  9, 13),
                new(31,  3, 10, 10, 14),
                new(16, 10, 11, 11, 15),
                new(31,  9, 13, 12, 13),
                new(31,  3, 13, 13, 14),
                new(31, 10, 13, 14, 15),
                new(31, 12, 13, 12, 15),
                new(7,  7,  7, 16, 17),
                new(7,  7,  7, 18, 19),
                new(7,  7,  7, 19, 20),
                new(7,  7,  7, 18, 20),
                new(7,  7,  7, 20, 21),
                new(7,  6,  6, 22, 23),
                new(7,  6,  6, 23, 24),
                new(7,  6,  6, 24, 22),
                new(7,  6,  6, 25, 26),
                new(7,  6,  6, 26, 27),
                new(7,  6,  6, 25, 27),
                new(7,  6,  6, 27, 28),
                new(6,  3,  3, 29, 30),
                new(6,  3,  3, 31, 32),
                new(8,  0,  0, 33, 34),
                new(5,  0,  0, 34, 35),
                new(5,  0,  0, 35, 36),
                new(5,  0,  0, 36, 33),
            };
            Name = "Transporter";
            Points = new ShipPoint[]
            {
                new(new(0,   10,  -26), 31,  0,  6,  7,  7),
                new(new(-25,    4,  -26), 31,  0,  1,  7,  7),
                new(new(-28,   -3,  -26), 31,  0,  1,  2,  2),
                new(new(-25,   -8,  -26), 31,  0,  2,  3,  3),
                new(new(26,   -8,  -26), 31,  0,  3,  4,  4),
                new(new(29,   -3,  -26), 31,  0,  4,  5,  5),
                new(new(26,    4,  -26), 31,  0,  5,  6,  6),
                new(new(0,    6,   12), 19, 15, 15, 15, 15),
                new(new(-30,   -1,   12), 31,  1,  7,  8,  9),
                new(new(-33,   -8,   12), 31,  1,  2,  3,  9),
                new(new(33,   -8,   12), 31,  3,  4,  5, 10),
                new(new(30,   -1,   12), 31,  5,  6, 10, 11),
                new(new(-11,   -2,   30), 31,  8,  9, 12, 13),
                new(new(-13,   -8,   30), 31,  3,  9, 13, 13),
                new(new(14,   -8,   30), 31,  3, 10, 13, 13),
                new(new(11,   -2,   30), 31, 10, 11, 12, 13),
                new(new(-5,    6,    2),  7,  7,  7,  7,  7),
                new(new(-18,    3,    2),  7,  7,  7,  7,  7),
                new(new(-5,    7,   -7),  7,  7,  7,  7,  7),
                new(new(-18,    4,   -7),  7,  7,  7,  7,  7),
                new(new(-11,    6,  -14),  7,  7,  7,  7,  7),
                new(new(-11,    5,   -7),  7,  7,  7,  7,  7),
                new(new(5,    7,  -14),  7,  6,  6,  6,  6),
                new(new(18,    4,  -14),  7,  6,  6,  6,  6),
                new(new(11,    5,   -7),  7,  6,  6,  6,  6),
                new(new(5,    6,   -3),  7,  6,  6,  6,  6),
                new(new(18,    3,   -3),  7,  6,  6,  6,  6),
                new(new(11,    4,    8),  7,  6,  6,  6,  6),
                new(new(11,    5,   -3),  7,  6,  6,  6,  6),
                new(new(-16,   -8,  -13),  6,  3,  3,  3,  3),
                new(new(-16,   -8,   16),  6,  3,  3,  3,  3),
                new(new(17,   -8,  -13),  6,  3,  3,  3,  3),
                new(new(17,   -8,   16),  6,  3,  3,  3,  3),
                new(new(-13,   -3,  -26),  8,  0,  0,  0,  0),
                new(new(13,   -3,  -26),  8,  0,  0,  0,  0),
                new(new(9,    3,  -26),  5,  0,  0,  0,  0),
                new(new(-8,    3,  -26),  5,  0,  0,  0,  0),
            };
            Size = 2500;
            Class = ShipClass.SpaceJunk;
            VanishPoint = 16;
            VelocityMax = 10;
        }
    }
}
