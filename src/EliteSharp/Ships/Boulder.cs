// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class Boulder : NullObject
    {
        internal Boulder()
        {
            Type = ShipType.Boulder;
            Flags = ShipFlags.Inactive;
            Bounty = 0.1f;
            EnergyMax = 20;
            FaceNormals = new ShipFaceNormal[]
            {
                new(31, new(-15,   -3,    8)),
                new(31, new(-7,   12,   30)),
                new(31, new(32,  -47,   24)),
                new(31, new(-3,  -39,   -7)),
                new(31, new(-5,   -4,   -1)),
                new(31, new(49,   84,    8)),
                new(31, new(112,   21,  -21)),
                new(31, new(76,  -35,  -82)),
                new(31, new(22,   56, -137)),
                new(31, new(40,  110,  -38)),
            };
            Faces = new ShipFace[]
            {
                new(Colour.DarkerGrey, new(-0x0F, -0x03, 0x08), new[] { 0, 4, 5 }),
                new(Colour.LightGrey, new(-0x07, 0x0C, 0x1E), new[] { 0, 5, 1 }),
                new(Colour.DarkGrey, new(0x20, -0x2F, 0x18), new[] { 1, 5, 2 }),
                new(Colour.DarkerGrey, new(-0x03, -0x27, -0x07), new[] { 2, 5, 3 }),
                new(Colour.LightGrey, new(-0x05, -0x04, -0x01), new[] { 3, 5, 4 }),

                new(Colour.DarkGrey, new(0x31, 0x54, 0x08), new[] { 1, 6, 0 }),
                new(Colour.DarkerGrey, new(0x70, 0x15, -0x15), new[] { 2, 6, 1 }),
                new(Colour.LightGrey, new(0x4C, -0x23, -0x52), new[] { 3, 6, 2 }),
                new(Colour.DarkGrey, new(0x16, 0x38, -0x89), new[] { 4, 6, 3 }),
                new(Colour.LightGrey, new(0x28, 0x6E, -0x26), new[] { 6, 4, 0 }),
            };
            Lines = new ShipLine[]
            {
                new(31,  1,  5,  0,  1),
                new(31,  2,  6,  1,  2),
                new(31,  3,  7,  2,  3),
                new(31,  4,  8,  3,  4),
                new(31,  0,  9,  4,  0),
                new(31,  0,  1,  0,  5),
                new(31,  1,  2,  1,  5),
                new(31,  2,  3,  2,  5),
                new(31,  3,  4,  3,  5),
                new(31,  0,  4,  4,  5),
                new(31,  5,  9,  0,  6),
                new(31,  5,  6,  1,  6),
                new(31,  6,  7,  2,  6),
                new(31,  7,  8,  3,  6),
                new(31,  8,  9,  4,  6),
            };
            Name = "Boulder";
            Points = new ShipPoint[]
            {
                new(new(-18,   37,  -11), 31,  0,  1,  5,  9),
                new(new(30,    7,   12), 31,  1,  2,  5,  6),
                new(new(28,   -7,  -12), 31,  2,  3,  6,  7),
                new(new(2,    0,  -39), 31,  3,  4,  7,  8),
                new(new(-28,   34,  -30), 31,  0,  4,  8,  9),
                new(new(5,  -10,   13), 31, 15, 15, 15, 15),
                new(new(20,   17,  -30), 31, 15, 15, 15, 15),
            };
            Size = 900;
            Class = ShipClass.SpaceJunk;
            VanishPoint = 20;
            VelocityMax = 30;
        }
    }
}
