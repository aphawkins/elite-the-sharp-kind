// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Enums;

namespace EliteSharp.Ships
{
    internal sealed class Missile : NullObject
    {
        internal Missile()
        {
            Type = ShipType.Missile;
            EnergyMax = 2;
            FaceNormals = new ShipFaceNormal[]
            {
                new(31, new(-64,    0,   16)),
                new(31, new(0,  -64,   16)),
                new(31, new(64,    0,   16)),
                new(31, new(0,   64,   16)),
                new(31, new(32,    0,    0)),
                new(31, new(0,  -32,    0)),
                new(31, new(-32,    0,    0)),
                new(31, new(0,   32,    0)),
                new(31, new(0,    0, -176)),
            };
            Faces = new ShipFace[]
            {
                //fins
                new(Colour.LighterRed, new(0x20, 0x00, 0x00), new[] { 5, 9, 15 }),
                new(Colour.LighterRed, new(0x00, 0x20, 0x00), new[] { 15, 9,  5 }),

                new(Colour.LighterRed, new(-0x20, 0x00, 0x00), new[] { 8, 12, 13 }),
                new(Colour.LighterRed, new(0x00, 0x20, 0x00), new[] { 13, 12, 8 }),

                new(Colour.LighterRed, new(-0x20, 0x00, 0x00), new[] { 7, 11, 14 }),
                new(Colour.LighterRed, new(0x00, -0x20, 0x00), new[] { 14, 11, 7 }),

                new(Colour.LighterRed, new(0x20, 0x00, 0x00), new[] { 6, 10, 16 }),
                new(Colour.LighterRed, new(0x00, -0x20, 0x00), new[] { 16, 10, 6 }),

                //nose cone
                new(Colour.LightRed, new(-0x40, 0x00, 0x10), new[] { 0,  3,  4 }),
                new(Colour.LighterRed,      new(0x00, -0x40, 0x10), new[] { 0,  4,  1 }),
                new(Colour.LightRed, new(0x40, 0x00, 0x10), new[] { 0,  1,  2 }),
                new(Colour.LighterRed,      new(0x00, 0x40, 0x10), new[] { 0,  2,  3 }),

                //main body
                new(Colour.DarkerGrey, new(0x20, 0x00, 0x00), new[] { 6,  5,  2, 1 }),
                new(Colour.LightGrey, new(0x00, 0x20, 0x00), new[] { 5,  8,  3, 2 }),
                new(Colour.DarkerGrey, new(-0x20, 0x00, 0x00), new[] { 8,  7,  4, 3 }),
                new(Colour.LightGrey, new(0x00, -0x20, 0x00), new[] { 7,  6,  1, 4 }),

                //bottom
                new(Colour.DarkGrey, new(0x00, 0x00, -0xB0), new[] { 5,  6,  7, 8 }),
            };
            Lines = new ShipLine[]
            {
                new(31,  2,  1,  0,  1),
                new(31,  3,  2,  0,  2),
                new(31,  3,  0,  0,  3),
                new(31,  1,  0,  0,  4),
                new(31,  2,  4,  1,  2),
                new(31,  5,  1,  1,  4),
                new(31,  6,  0,  3,  4),
                new(31,  7,  3,  2,  3),
                new(31,  7,  4,  2,  5),
                new(31,  5,  4,  1,  6),
                new(31,  6,  5,  4,  7),
                new(31,  7,  6,  3,  8),
                new(31,  8,  6,  7,  8),
                new(31,  8,  7,  5,  8),
                new(31,  8,  4,  5,  6),
                new(31,  8,  5,  6,  7),
                new(8,  8,  5,  6, 10),
                new(8,  8,  7,  5,  9),
                new(8,  8,  7,  8, 12),
                new(8,  8,  5,  7, 11),
                new(8,  7,  4,  9, 15),
                new(8,  5,  4, 10, 16),
                new(8,  7,  6, 12, 13),
                new(8,  6,  5, 11, 14),
            };
            Name = "Missile";
            Points = new ShipPoint[]
            {
                new(new(0,    0,   68), 31,  1,  0,  3,  2),
                new(new(8,   -8,   36), 31,  2,  1,  5,  4),
                new(new(8,    8,   36), 31,  3,  2,  7,  4),
                new(new(-8,    8,   36), 31,  3,  0,  7,  6),
                new(new(-8,   -8,   36), 31,  1,  0,  6,  5),
                new(new(8,    8,  -44), 31,  7,  4,  8,  8),
                new(new(8,   -8,  -44), 31,  5,  4,  8,  8),
                new(new(-8,   -8,  -44), 31,  6,  5,  8,  8),
                new(new(-8,    8,  -44), 31,  7,  6,  8,  8),
                new(new(12,   12,  -44),  8,  7,  4,  8,  8),
                new(new(12,  -12,  -44),  8,  5,  4,  8,  8),
                new(new(-12,  -12,  -44),  8,  6,  5,  8,  8),
                new(new(-12,   12,  -44),  8,  7,  6,  8,  8),
                new(new(-8,    8,  -12),  8,  7,  6,  7,  7),
                new(new(-8,   -8,  -12),  8,  6,  5,  6,  6),
                new(new(8,    8,  -12),  8,  7,  4,  7,  7),
                new(new(8,   -8,  -12),  8,  5,  4,  5,  5),
            };
            Size = 1600;
            Class = ShipClass.Missile;
            VanishPoint = 14;
            VelocityMax = 44;
        }
    }
}