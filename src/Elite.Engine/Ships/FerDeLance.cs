// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class FerDeLance : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 160;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(28, new(   0,   24,    6)),
            new(31, new( -68,    0,   24)),
            new(31, new( -63,    0,  -37)),
            new(31, new(   0,    0, -104)),
            new(31, new(  63,    0,  -37)),
            new(31, new(  68,    0,   24)),
            new(28, new( -12,   46,  -19)),
            new(28, new(   0,   45,  -22)),
            new(28, new(  12,   46,  -19)),
            new(31, new(   0,  -28,    0)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.Grey1, new( 0x00, 0x18, 0x06), new[] { 5,  0,  8, 9 }),
            new(Colour.Grey2,new( -0x44, 0x00, 0x18), new[] {   0,  5,  1 }),

            new (Colour.Blue2, new(-0x3F, 0x00,-0x25), new[] {   2,  1,  5, 6 }),

            new (Colour.Red1, new( 0x00, 0x00,-0x68), new[] {   3,  2,  6, 7 }),

            new (Colour.Blue2, new( 0x3F, 0x00,-0x25), new[] { 4,  3,  7, 8 }),
            new (Colour.Grey2, new( 0x44, 0x00, 0x18), new[] { 4,  8,  0 }),

            new (Colour.Blue3, new(-0x0C, 0x2E,-0x13), new[] { 5,  9,  6 }),
            new (Colour.Blue2, new( 0x00, 0x2D,-0x16), new[] { 6,  9,  7 }),
            new (Colour.Blue3, new( 0x0C, 0x2E,-0x13), new[] { 7,  9,  8 }),
            new (Colour.Grey3, new( 0x00,-0x1C, 0x00), new[] { 4,  0,  1, 2, 3 }),

            new (Colour.Red2, new( 0x00,-0x1C, 0x00), new[] { 16, 18, 17 }),
            new (Colour.Red2, new( 0x00, 0x18, 0x06), new[] { 11, 10, 12 }),
            new (Colour.Red2, new( 0x00, 0x18, 0x06), new[] { 15, 13, 14 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 9;

        public ShipLine[] Lines { get; } =
        {
            new(31,  1,  9,  0,  1),
            new(31,  2,  9,  1,  2),
            new(31,  3,  9,  2,  3),
            new(31,  4,  9,  3,  4),
            new(31,  5,  9,  0,  4),
            new(28,  0,  1,  0,  5),
            new(28,  2,  6,  5,  6),
            new(28,  3,  7,  6,  7),
            new(28,  4,  8,  7,  8),
            new(28,  0,  5,  0,  8),
            new(15,  0,  6,  5,  9),
            new(11,  6,  7,  6,  9),
            new(11,  7,  8,  7,  9),
            new(15,  0,  8,  8,  9),
            new(14,  1,  2,  1,  5),
            new(14,  2,  3,  2,  6),
            new(14,  3,  4,  3,  7),
            new(14,  4,  5,  4,  8),
            new( 8,  0,  0, 10, 11),
            new( 9,  0,  0, 11, 12),
            new(11,  0,  0, 10, 12),
            new( 8,  0,  0, 13, 14),
            new( 9,  0,  0, 14, 15),
            new(11,  0,  0, 13, 15),
            new(12,  9,  9, 16, 17),
            new(12,  9,  9, 16, 18),
            new( 8,  9,  9, 17, 18),
        };

        public int LootMax => 0;

        public int MissilesMax => 2;

        public string Name => "Fer-de-Lance";

        public ShipPoint[] Points { get; } =
                                                                                        {
            new(new(   0,  -14,  108), 31,  0,  1,  5,  9),
            new(new( -40,  -14,   -4), 31,  1,  2,  9,  9),
            new(new( -12,  -14,  -52), 31,  2,  3,  9,  9),
            new(new(  12,  -14,  -52), 31,  3,  4,  9,  9),
            new(new(  40,  -14,   -4), 31,  4,  5,  9,  9),
            new(new( -40,   14,   -4), 28,  0,  1,  2,  6),
            new(new( -12,    2,  -52), 28,  2,  3,  6,  7),
            new(new(  12,    2,  -52), 28,  3,  4,  7,  8),
            new(new(  40,   14,   -4), 28,  0,  4,  5,  8),
            new(new(   0,   18,  -20), 15,  0,  6,  7,  8),
            new(new(  -3,  -11,   97), 11,  0,  0,  0,  0),
            new(new( -26,    8,   18),  9,  0,  0,  0,  0),
            new(new( -16,   14,   -4), 11,  0,  0,  0,  0),
            new(new(   3,  -11,   97), 11,  0,  0,  0,  0),
            new(new(  26,    8,   18),  9,  0,  0,  0,  0),
            new(new(  16,   14,   -4), 11,  0,  0,  0,  0),
            new(new(   0,  -14,  -20), 12,  9,  9,  9,  9),
            new(new( -14,  -14,   44), 12,  9,  9,  9,  9),
            new(new(  14,  -14,   44), 12,  9,  9,  9,  9),
        };

        public StockType ScoopedType => StockType.None;
        public float Size => 1600;
        public ShipClass Type => throw new NotImplementedException();
        public int VanishPoint => 40;

        public float VelocityMax => 30;
    }
}
