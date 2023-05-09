// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal sealed class Viper : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 140;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(   0,   32,    0)),
            new(31, new( -22,   33,   11)),
            new(31, new(  22,   33,   11)),
            new(31, new( -22,  -33,   11)),
            new(31, new(  22,  -33,   11)),
            new(31, new(   0,  -32,    0)),
            new(31, new(   0,    0,  -48)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(GFX_COL.GFX_COL_GREY_2, new( 0x00, 0x20, 0x00), new[] { 7,  8,  1 }),

            new(GFX_COL.GFX_COL_BLUE_3, new(-0x16, 0x21, 0x0B), new[] { 8,  4,  0, 1 }),
            new(GFX_COL.GFX_COL_BLUE_2, new( 0x16, 0x21, 0x0B), new[] { 3,  7,  1, 0 }),

            new(GFX_COL.GFX_COL_BLUE_2, new(-0x16,-0x21, 0x0B), new[] { 2,  0,  4, 6 }),
            new(GFX_COL.GFX_COL_BLUE_3, new( 0x16,-0x21, 0x0B), new[] { 0,  2,  5, 3 }),

            new(GFX_COL.GFX_COL_GREY_2, new( 0x00,-0x20, 0x00), new[] { 2,  6, 5 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x00, 0x00,-0x30), new[] { 4,  8,  7, 3, 5, 6 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00, 0x00,-0x30), new[] { 12, 13, 9 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00, 0x00,-0x30), new[] { 10, 14, 11 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 8;

        public ShipLine[] Lines { get; } =
        {
            new(31,  4,  2,  0,  3),
            new(30,  2,  1,  0,  1),
            new(30,  4,  3,  0,  2),
            new(31,  3,  1,  0,  4),
            new(30,  2,  0,  1,  7),
            new(30,  1,  0,  1,  8),
            new(30,  5,  4,  2,  5),
            new(30,  5,  3,  2,  6),
            new(31,  6,  0,  7,  8),
            new(30,  6,  5,  5,  6),
            new(31,  6,  1,  4,  8),
            new(30,  6,  3,  4,  6),
            new(31,  6,  2,  3,  7),
            new(30,  4,  6,  3,  5),
            new(19,  6,  6,  9, 12),
            new(18,  6,  6,  9, 13),
            new(19,  6,  6, 10, 11),
            new(18,  6,  6, 10, 14),
            new(16,  6,  6, 11, 14),
            new(16,  6,  6, 12, 13),
        };

        public int LootMax => 0;

        public int MissilesMax => 1;

        public string Name => "Viper";

        public ShipPoint[] Points { get; } =
                                                                                        {
            new(new(   0,    0,   72), 31,  2,  1,  4,  3),
            new(new(   0,   16,   24), 30,  1,  0,  2,  2),
            new(new(   0,  -16,   24), 30,  4,  3,  5,  5),
            new(new(  48,    0,  -24), 31,  4,  2,  6,  6),
            new(new( -48,    0,  -24), 31,  3,  1,  6,  6),
            new(new(  24,  -16,  -24), 30,  5,  4,  6,  6),
            new(new( -24,  -16,  -24), 30,  3,  5,  6,  6),
            new(new(  24,   16,  -24), 31,  2,  0,  6,  6),
            new(new( -24,   16,  -24), 31,  1,  0,  6,  6),
            new(new( -32,    0,  -24), 19,  6,  6,  6,  6),
            new(new(  32,    0,  -24), 19,  6,  6,  6,  6),
            new(new(   8,    8,  -24), 19,  6,  6,  6,  6),
            new(new(  -8,    8,  -24), 19,  6,  6,  6,  6),
            new(new(  -8,   -8,  -24), 18,  6,  6,  6,  6),
            new(new(   8,   -8,  -24), 18,  6,  6,  6,  6),
        };
        public StockType ScoopedType => StockType.None;
        public float Size => 5625;
        public ShipClass Type => ShipClass.Police;
        public int VanishPoint => 23;

        public float VelocityMax => 32;
    }
}
