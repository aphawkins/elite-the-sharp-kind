// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Views;

namespace EliteSharp.Ships
{
    internal sealed class CobraMk1 : ShipBase
    {
        internal CobraMk1(IDraw draw)
            : base(draw)
        {
            Type = ShipType.CobraMk1;
            Flags = ShipFlags.Bold | ShipFlags.Angry;
            Bounty = 7.5f;
            EnergyMax = 90;
            FaceNormals = new ShipFaceNormal[]
            {
                new(31, new(0,   41,   10)),
                new(31, new(0,  -27,    3)),
                new(31, new(-8,   46,    8)),
                new(31, new(-12,  -57,   12)),
                new(31, new(8,   46,    8)),
                new(31, new(12,  -57,   12)),
                new(31, new(0,   49,    0)),
                new(31, new(0,    0, -154)),
                new(31, new(-121,  111,  -62)),
                new(31, new(121,  111,  -62)),
            };
            Faces = new ShipFace[]
            {
                new(Colour.Blue, new(0x00, 0x29, 0x0A), new[] { 0, 1, 8 }),
                new(Colour.Blue, new(0x00, -0x1B, 0x03), new[] { 6, 7, 1, 0 }),

                new(Colour.DarkBlue, new(-0x08, 0x2E, 0x08), new[] { 2, 0, 8, 4 }),
                new(Colour.Purple, new(-0x0C, -0x39, 0x0C), new[] { 6, 0, 2 }),
                new(Colour.DarkBlue, new(0x08, 0x2E, 0x08), new[] { 1, 3, 5, 8 }),
                new(Colour.Purple, new(0x0C, -0x39, 0x0C), new[] { 1, 7, 3 }),

                new(Colour.Blue, new(0x00, 0x31, 0x00), new[] { 4, 8, 5 }),
                new(Colour.Purple, new(0x00, 0x00, -0x9A), new[] { 7, 6, 4, 5 }),

                new(Colour.Blue, new(-0x79, 0x6F, -0x3E), new[] { 2, 4, 6 }),
                new(Colour.Blue, new(0x79, 0x6F, -0x3E), new[] { 3, 7, 5 }),

                new(Colour.White, new(0x00, 0x29, 0x0A), new[] { 9, 10 }),
                new(Colour.White, new(0x00, -0x1B, 0x03), new[] { 10, 9 }),
            };
            LaserFront = 10;
            LaserStrength = 9;
            Lines = new ShipLine[]
            {
                new(31,  0,  1,  1,  0),
                new(31,  2,  3,  0,  2),
                new(31,  3,  8,  2,  6),
                new(31,  1,  7,  6,  7),
                new(31,  5,  9,  7,  3),
                new(31,  4,  5,  3,  1),
                new(31,  2,  8,  2,  4),
                new(31,  6,  7,  4,  5),
                new(31,  4,  9,  5,  3),
                new(20,  0,  2,  0,  8),
                new(20,  0,  4,  8,  1),
                new(16,  2,  6,  4,  8),
                new(16,  4,  6,  8,  5),
                new(31,  7,  8,  4,  6),
                new(31,  7,  9,  5,  7),
                new(20,  1,  3,  0,  6),
                new(20,  1,  5,  1,  7),
                new(2,  0,  1, 10,  9),
            };
            LootMax = 3;
            MinDistance = 384;
            MissilesMax = 2;
            Name = "Cobra MkI";
            Points = new ShipPoint[]
            {
                new(new(-18,   -1,   50), 31,  0,  1,  2,  3),
                new(new(18,   -1,   50), 31,  0,  1,  4,  5),
                new(new(-66,    0,    7), 31,  2,  3,  8,  8),
                new(new(66,    0,    7), 31,  4,  5,  9,  9),
                new(new(-32,   12,  -38), 31,  2,  6,  7,  8),
                new(new(32,   12,  -38), 31,  4,  6,  7,  9),
                new(new(-54,  -12,  -38), 31,  1,  3,  7,  8),
                new(new(54,  -12,  -38), 31,  1,  5,  7,  9),
                new(new(0,   12,   -6), 20,  0,  2,  4,  6),
                new(new(0,   -1,   50),  2,  0,  1,  1,  1),
                new(new(0,   -1,   60), 31,  0,  1,  1,  1),
            };
            Size = 9801;
            Class = ShipClass.PackHunter;
            VanishPoint = 19;
            VelocityMax = 26;
        }
    }
}
