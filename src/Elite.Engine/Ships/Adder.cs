// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal sealed class Adder : Ship
    {
        public override float Bounty => 4;

        public override int EnergyMax => 85;

        public override ShipFaceNormal[] FaceNormals { get; protected set; } =
        {
            new(31, new(0,   39,   10)),
            new(31, new(0,  -39,   10)),
            new(31, new(69,   50,   13)),
            new(31, new(69,  -50,   13)),
            new(31, new(30,   52,    0)),
            new(31, new(30,  -52,    0)),
            new(31, new(0,    0, -160)),
            new(31, new(0,    0, -160)),
            new(31, new(0,    0, -160)),
            new(31, new(-30,   52,    0)),
            new(31, new(-30,  -52,    0)),
            new(31, new(-69,   50,   13)),
            new(31, new(-69,  -50,   13)),
            new(31, new(0,   28,    0)),
            new(31, new(0,  -28,    0)),
        };

        public override ShipFace[] Faces { get; protected set; } =
        {
            new(Colour.LightGrey, new(0x00, 0x27, 0x0A), new[] { 0, 1,  11,  10 }),
            new(Colour.LightGrey, new(0x00, -0x27, 0x0A), new[] { 1, 0,  12,  13 }),

            new(Colour.RedOrange,    new(0x45, 0x32, 0x0D), new[] { 2, 11,  1 }),
            new(Colour.LighterRed,      new(0x45, -0x32, 0x0D), new[] { 1, 13,  2 }),
            new(Colour.LightRed, new(0x1E, 0x34, 0x00), new[] { 9, 11,  2,  3 }),
            new(Colour.Red,    new(0x1E, -0x34, 0x00), new[] { 3, 2, 13,  4 }),

            new(Colour.LightRed, new(-0x1E, 0x34, 0x00), new[] { 10,  8,  6,  7 }),
            new(Colour.Red,    new(-0x1E, -0x34, 0x00), new[] { 7,  6,  5, 12 }),
            new(Colour.RedOrange,    new(-0x45, 0x32, 0x0D), new[] { 10,  7, 0 }),
            new(Colour.LighterRed,      new(-0x45, -0x32, 0x0D), new[] { 0,  7, 12 }),

            new(Colour.DarkerGrey, new(0x00, 0x00, -0xA0), new[] { 3,  4,  5,  6, 8, 9 }),
            new(Colour.DarkGrey, new(0x00, 0x1C, 0x00), new[] { 10, 11,  9, 8 }),
            new(Colour.DarkGrey, new(0x00, -0x1C, 0x00), new[] { 5, 4,  13, 12 }),
            new(Colour.LightBlue, new(0x00, 0x27, 0x0A), new[] { 17, 14, 15, 16 }),
        };

        public override int LaserStrength => 8;

        public override ShipLine[] Lines { get; protected set; } =
        {
            new(31,  0,  1,  0,  1),
            new(7,  2,  3,  1,  2),
            new(31,  4,  5,  2,  3),
            new(31,  5,  6,  3,  4),
            new(31,  7, 14,  4,  5),
            new(31,  8, 10,  5,  6),
            new(31,  9, 10,  6,  7),
            new(7, 11, 12,  7,  0),
            new(31,  4,  6,  3,  9),
            new(31,  7, 13,  9,  8),
            new(31,  8,  9,  8,  6),
            new(31,  0, 11,  0, 10),
            new(31,  9, 11,  7, 10),
            new(31,  0,  2,  1, 11),
            new(31,  2,  4,  2, 11),
            new(31,  1, 12,  0, 12),
            new(31, 10, 12,  7, 12),
            new(31,  1,  3,  1, 13),
            new(31,  3,  5,  2, 13),
            new(31,  0, 13, 10, 11),
            new(31,  1, 14, 12, 13),
            new(31,  9, 13,  8, 10),
            new(31,  4, 13,  9, 11),
            new(31, 10, 14,  5, 12),
            new(31,  5, 14,  4, 13),
            new(5,  0,  0, 14, 15),
            new(3,  0,  0, 15, 16),
            new(4,  0,  0, 16, 17),
            new(3,  0,  0, 17, 14),
        };

        public override string Name => "Adder";

        public override ShipPoint[] Points { get; protected set; } =
        {
            new(new(-18,    0,   40), 31,  0,  1, 11, 12),
            new(new(18,    0,   40), 31,  0,  1,  2,  3),
            new(new(30,    0,  -24), 31,  2,  3,  4,  5),
            new(new(30,    0,  -40), 31,  4,  5,  6,  6),
            new(new(18,   -7,  -40), 31,  5,  6,  7, 14),
            new(new(-18,   -7,  -40), 31,  7,  8, 10, 14),
            new(new(-30,    0,  -40), 31,  8,  9, 10, 10),
            new(new(-30,    0,  -24), 31,  9, 10, 11, 12),
            new(new(-18,    7,  -40), 31,  7,  8,  9, 13),
            new(new(18,    7,  -40), 31,  4,  6,  7, 13),
            new(new(-18,    7,   13), 31,  0,  9, 11, 13),
            new(new(18,    7,   13), 31,  0,  2,  4, 13),
            new(new(-18,   -7,   13), 31,  1, 10, 12, 14),
            new(new(18,   -7,   13), 31,  1,  3,  5, 14),
            new(new(-11,    3,   29),  5,  0,  0,  0,  0),
            new(new(11,    3,   29),  5,  0,  0,  0,  0),
            new(new(11,    4,   24),  4,  0,  0,  0,  0),
            new(new(-11,    4,   24),  4,  0,  0,  0,  0),
        };

        public override float Size => 2500;

        public override ShipClass Class => ShipClass.PackHunter;

        public override int VanishPoint => 20;

        public override float VelocityMax => 24;
    }
}
