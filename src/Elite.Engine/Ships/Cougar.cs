// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Cougar : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 252;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(-16,   46,    4)),
            new(31, new(-16,  -46,    4)),
            new(31, new(0,  -27,    5)),
            new(31, new(16,  -46,    4)),
            new(31, new(16,   46,    4)),
            new(30, new(0,    0, -160)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.LightGrey, new(-0x10,  0x2E,  0x04), new[] { 2,  1,  0, 3 }),
            new(Colour.DarkGrey, new(-0x10, -0x2E,  0x04), new[] { 4,  1,  2 }),
            new(Colour.Grey, new(0x00, -0x1B,  0x05), new[] { 4,  5,  0, 1 }),
            new(Colour.DarkGrey, new(0x10, -0x2E,  0x04), new[] { 6,  5, 4 }),
            new(Colour.DarkGrey, new(0x10,  0x2E,  0x04), new[] { 5,  6,  3, 0 }),
            new(Colour.DarkerGrey, new(0x00,  0x00, -0xA0), new[] { 6,  4,  2, 3 }),

            new(Colour.DarkYellow, new(-0x10, -0x2E,  0x04), new[] { 1,  2,  8, 7 }),
            new(Colour.DarkYellow, new(-0x10,  0x2E,  0x04), new[] { 7,  8,  2, 1 }),
            new(Colour.DarkYellow, new(0x10,  0x2E,  0x04), new[] { 5,  6, 10, 9 }),
            new(Colour.DarkYellow, new(0x10, -0x2E,  0x04), new[] { 9, 10,  6, 5 }),

            new(Colour.DarkBlue, new(-0x10,  0x2E,  0x04), new[] { 12, 13, 11 }),
            new(Colour.Blue, new(0x10,  0x2E,  0x04), new[] { 11, 14, 12 }),

            //new(8,     0x00,  0x00, -0xA0, 3, 15, 16, 19, 0, 0, 0, 0, 0),
            //new(8,     0x00,  0x00, -0xA0, 3, 19, 18, 17, 0, 0, 0, 0, 0),
        };

        public int LaserFront => 0;

        public int LaserStrength => 26;

        public ShipLine[] Lines { get; } =
        {
            new(31,  0,  2,  0,  1),
            new(31,  0,  1,  1,  7),
            new(31,  0,  1,  7,  8),
            new(31,  0,  1,  8,  2),
            new(30,  0,  5,  2,  3),
            new(30,  4,  5,  3,  6),
            new(30,  1,  5,  2,  4),
            new(30,  3,  5,  4,  6),
            new(31,  3,  4,  6, 10),
            new(31,  3,  4, 10,  9),
            new(31,  3,  4,  9,  5),
            new(31,  2,  4,  5,  0),
            new(27,  0,  4,  0,  3),
            new(27,  1,  2,  1,  4),
            new(27,  2,  3,  5,  4),
            new(26,  0,  1,  1,  2),
            new(26,  3,  4,  5,  6),
            new(20,  0,  0, 12, 13),
            new(18,  0,  0, 13, 11),
            new(18,  4,  4, 11, 14),
            new(20,  4,  4, 14, 12),
            new(18,  5,  5, 15, 16),
            new(20,  5,  5, 16, 18),
            new(18,  5,  5, 18, 17),
            new(20,  5,  5, 17, 15),
        };

        public int LootMax => 3;

        public int MissilesMax => 4;

        public string Name => "Cougar";

        public ShipPoint[] Points { get; } =
        {
            new(new(0,    5,   67), 31,  0,  2,  4,  4),
            new(new(-20,    0,   40), 31,  0,  1,  2,  2),
            new(new(-40,    0,  -40), 31,  0,  1,  5,  5),
            new(new(0,   14,  -40), 30,  0,  4,  5,  5),
            new(new(0,  -14,  -40), 30,  1,  2,  3,  5),
            new(new(20,    0,   40), 31,  2,  3,  4,  4),
            new(new(40,    0,  -40), 31,  3,  4,  5,  5),
            new(new(-36,    0,   56), 31,  0,  1,  1,  1),
            new(new(-60,    0,  -20), 31,  0,  1,  1,  1),
            new(new(36,    0,   56), 31,  3,  4,  4,  4),
            new(new(60,    0,  -20), 31,  3,  4,  4,  4),
            new(new(0,    7,   35), 18,  0,  0,  4,  4),
            new(new(0,    8,   25), 20,  0,  0,  4,  4),
            new(new(-12,    2,   45), 20,  0,  0,  0,  0),
            new(new(12,    2,   45), 20,  4,  4,  4,  4),
            new(new(-10,    6,  -40), 20,  5,  5,  5,  5),
            new(new(-10,   -6,  -40), 20,  5,  5,  5,  5),
            new(new(10,   -6,  -40), 20,  5,  5,  5,  5),
            new(new(10,    6,  -40), 20,  5,  5,  5,  5),
        };

        public StockType ScoopedType => StockType.None;

        public float Size => 4900;

        public ShipClass Type => ShipClass.LoneWolf;

        public int VanishPoint => 34;

        public float VelocityMax => 40;
    }
}
