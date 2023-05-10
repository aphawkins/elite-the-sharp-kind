// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class CargoCannister : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 17;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(96,    0,    0)),
            new(31, new(0,   41,   30)),
            new(31, new(0,  -18,   48)),
            new(31, new(0,  -51,    0)),
            new(31, new(0,  -18,  -48)),
            new(31, new(0,   41,  -30)),
            new(31, new(-96,    0,    0)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.Grey4, new(0x60, 0x00, 0x00), new[] { 4, 0, 1, 2, 3 }),

            new(Colour.Grey2, new(0x00, 0x29, 0x1E), new[] { 5, 6, 1, 0 }),
            new(Colour.Grey1, new(0x00, -0x12, 0x30), new[] { 6, 7, 2, 1 }),
            new(Colour.Grey3, new(0x00, -0x33, 0x00), new[] { 7, 8, 3, 2 }),
            new(Colour.Grey1, new(0x00, -0x12, -0x30), new[] { 8, 9, 4, 3 }),
            new(Colour.Grey3, new(0x00, 0x29, -0x1E), new[] { 9, 5, 0, 4 }),

            new(Colour.Grey4, new(-0x60, 0x00, 0x00), new[] { 8, 7, 6, 5, 9 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 0;

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

        public string Name => "Cargo Cannister";

        public ShipPoint[] Points { get; } =
        {
            new(new(24,   16,    0), 31,  1,  0,  5,  5),
            new(new(24,    5,   15), 31,  1,  0,  2,  2),
            new(new(24,  -13,    9), 31,  2,  0,  3,  3),
            new(new(24,  -13,   -9), 31,  3,  0,  4,  4),
            new(new(24,    5,  -15), 31,  4,  0,  5,  5),
            new(new(-24,   16,    0), 31,  5,  1,  6,  6),
            new(new(-24,    5,   15), 31,  2,  1,  6,  6),
            new(new(-24,  -13,    9), 31,  3,  2,  6,  6),
            new(new(-24,  -13,   -9), 31,  4,  3,  6,  6),
            new(new(-24,    5,  -15), 31,  5,  4,  6,  6),
        };

        public StockType ScoopedType => StockType.None;
        public float Size => 400;
        public ShipClass Type => ShipClass.SpaceJunk;
        public int VanishPoint => 12;

        public float VelocityMax => 15;
    }
}
