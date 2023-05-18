// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Tharglet : IShip
    {
        public float Bounty => 5;

        public int EnergyMax => 20;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(-36,    0,    0)),
            new(31, new(20,   -5,    7)),
            new(31, new(46,  -42,  -14)),
            new(31, new(36,    0, -104)),
            new(31, new(46,   42,  -14)),
            new(31, new(20,    5,    7)),
            new(31, new(36,    0,    0)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.LightRed, new(-0x24, 0x00, 0x00), new[] { 3, 2, 1, 0, 4 }),

            new(Colour.LightGrey, new(0x14, -0x05, 0x07), new[] { 6, 5, 0, 1 }),
            new(Colour.DarkGrey, new(0x2E, -0x2A, -0x0E), new[] { 7, 6, 1, 2 }),
            new(Colour.Grey, new(0x24, 0x00, -0x68), new[] { 8, 7, 2, 3 }),
            new(Colour.DarkGrey, new(0x2E, 0x2A, -0x0E), new[] { 9, 8, 3, 4 }),
            new(Colour.DarkerGrey, new(0x14, 0x05, 0x07), new[] { 4, 0, 5, 9 }),

            new(Colour.LightRed, new(0x24, 0x00, 0x00), new[] { 9, 5, 6, 7, 8 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 8;

        public ShipLine[] Lines { get; } =
        {
            new(31,  1,  0,  0,  1),
            new(31,  2,  0,  1,  2),
            new(31,  3,  0,  2,  3),
            new(31,  4,  0,  3,  4),
            new(31,  5,  0,  0,  4),
            new(31,  5,  1,  0,  5),
            new(31,  2,  1,  1,  6),
            new(31,  3,  2,  2,  7),
            new(31,  4,  3,  3,  8),
            new(31,  5,  4,  4,  9),
            new(31,  6,  1,  5,  6),
            new(31,  6,  2,  6,  7),
            new(31,  6,  3,  7,  8),
            new(31,  6,  4,  8,  9),
            new(31,  6,  5,  9,  5),
        };

        public int LootMax => 0;

        public int MissilesMax => 0;

        public string Name => "Tharglet";

        public ShipPoint[] Points { get; } =
        {
            new(new(-9,    0,   40), 31,  0,  1,  5,  5),
            new(new(-9,  -38,   12), 31,  0,  1,  2,  2),
            new(new(-9,  -24,  -32), 31,  0,  2,  3,  3),
            new(new(-9,   24,  -32), 31,  0,  3,  4,  4),
            new(new(-9,   38,   12), 31,  0,  4,  5,  5),
            new(new(9,    0,   -8), 31,  1,  5,  6,  6),
            new(new(9,  -10,  -15), 31,  1,  2,  6,  6),
            new(new(9,   -6,  -26), 31,  2,  3,  6,  6),
            new(new(9,    6,  -26), 31,  3,  4,  6,  6),
            new(new(9,   10,  -15), 31,  4,  5,  6,  6),
        };

        public StockType ScoopedType => StockType.AlienItems;

        public float Size => 1600;

        public ShipClass Type => ShipClass.Tharglet;

        public int VanishPoint => 20;

        public float VelocityMax => 30;
    }
}
