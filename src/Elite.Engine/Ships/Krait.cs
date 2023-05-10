// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Krait : IShip
    {
        public float Bounty => 10;

        public int EnergyMax => 80;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(3,   24,    3)),
            new(31, new(3,  -24,    3)),
            new(31, new(-3,  -24,    3)),
            new(31, new(-3,   24,    3)),
            new(31, new(38,    0,  -77)),
            new(31, new(-38,    0,  -77)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.Blue3, new(0x03, 0x18, 0x03), new[] { 0,  3,  1 }),
            new(Colour.Blue2, new(0x03, -0x18, 0x03), new[] { 2,  3,  0 }),

            new(Colour.Blue3, new(-0x03, -0x18, 0x03), new[] { 0,  4,  2 }),
            new(Colour.Blue2, new(-0x03, 0x18, 0x03), new[] { 1,  4,  0 }),

            new(Colour.Grey3, new(0x26, 0x00, -0x4D), new[] { 3,  2,  1 }),
            new(Colour.Grey1, new(-0x26, 0x00, -0x4D), new[] { 4,  1,  2 }),

            new(Colour.White1, new(0x03, -0x18, 0x03), new[] { 3,  5 }),
            new(Colour.White1, new(0x03, 0x18, 0x03), new[] { 5,  3 }),
            new(Colour.White1, new(-0x03, 0x18, 0x03), new[] { 4,  6 }),
            new(Colour.White1, new(-0x03, -0x18, 0x03), new[] { 6,  4 }),

            new(Colour.Red1, new(0x26, 0x00, -0x4D), new[] { 12, 11, 13 }),
            new(Colour.Red1, new(-0x26, 0x00, -0x4D), new[] { 16, 14, 15 }),
            new(Colour.White1, new(0x03, 0x18, 0x03), new[] { 7, 10,  8 }),
            new(Colour.White1, new(-0x03, 0x18, 0x03), new[] { 8,  9,  7 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 8;

        public ShipLine[] Lines { get; } =
        {
            new(31,  0,  3,  0,  1),
            new(31,  1,  2,  0,  2),
            new(31,  0,  1,  0,  3),
            new(31,  2,  3,  0,  4),
            new(31,  3,  5,  1,  4),
            new(31,  2,  5,  4,  2),
            new(31,  1,  4,  2,  3),
            new(31,  0,  4,  3,  1),
            new(30,  0,  1,  3,  5),
            new(30,  2,  3,  4,  6),
            new(8,  4,  5,  1,  2),
            new(9,  0,  0,  7, 10),
            new(6,  0,  0,  8, 10),
            new(9,  3,  3,  7,  9),
            new(6,  3,  3,  8,  9),
            new(8,  4,  4, 11, 13),
            new(8,  4,  4, 13, 12),
            new(7,  4,  4, 12, 11),
            new(7,  5,  5, 14, 15),
            new(8,  5,  5, 15, 16),
            new(8,  5,  5, 16, 14),
        };

        public int LootMax => 1;

        public int MissilesMax => 0;

        public string Name => "Krait";

        public ShipPoint[] Points { get; } =
        {
            new(new(0,    0,   96), 31,  0,  1,  2,  3),
            new(new(0,   18,  -48), 31,  0,  3,  4,  5),
            new(new(0,  -18,  -48), 31,  1,  2,  4,  5),
            new(new(90,    0,   -3), 31,  0,  1,  4,  4),
            new(new(-90,    0,   -3), 31,  2,  3,  5,  5),
            new(new(90,    0,   87), 30,  0,  1,  1,  1),
            new(new(-90,    0,   87), 30,  2,  3,  3,  3),
            new(new(0,    5,   53),  9,  0,  0,  3,  3),
            new(new(0,    7,   38),  6,  0,  0,  3,  3),
            new(new(-18,    7,   19),  9,  3,  3,  3,  3),
            new(new(18,    7,   19),  9,  0,  0,  0,  0),
            new(new(18,   11,  -39),  8,  4,  4,  4,  4),
            new(new(18,  -11,  -39),  8,  4,  4,  4,  4),
            new(new(36,    0,  -30),  8,  4,  4,  4,  4),
            new(new(-18,   11,  -39),  8,  5,  5,  5,  5),
            new(new(-18,  -11,  -39),  8,  5,  5,  5,  5),
            new(new(-36,    0,  -30),  8,  5,  5,  5,  5),
        };

        public StockType ScoopedType => StockType.None;
        public float Size => 3600;
        public ShipClass Type => ShipClass.PackHunter;
        public int VanishPoint => 20;

        public float VelocityMax => 30;
    }
}
