// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Coriolis : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 240;

        public ShipFaceNormal[] FaceNormals { get; } =
                        {
            new(31, new(   0,    0,  160)),
            new(31, new( 107, -107,  107)),
            new(31, new( 107,  107,  107)),
            new(31, new(-107,  107,  107)),
            new(31, new(-107, -107,  107)),
            new(31, new(   0, -160,    0)),
            new(31, new( 160,    0,    0)),
            new(31, new(-160,    0,    0)),
            new(31, new(   0,  160,    0)),
            new(31, new(-107, -107, -107)),
            new(31, new( 107, -107, -107)),
            new(31, new( 107,  107, -107)),
            new(31, new(-107,  107, -107)),
            new(31, new(   0,    0, -160)),
        };
        public ShipFace[] Faces { get; } =
        {
            new(GFX_COL.GFX_COL_GREY_3, new( 0x6B,-0x6B, 0x6B), new[] {  4,  0,  3 }),
            new(GFX_COL.GFX_COL_GREY_3, new( 0x6B, 0x6B, 0x6B), new[] {  0,  5,  1 }),
            new(GFX_COL.GFX_COL_GREY_3, new(-0x6B, 0x6B, 0x6B), new[] {  1,  6,  2 }),
            new(GFX_COL.GFX_COL_GREY_3, new(-0x6B,-0x6B, 0x6B), new[] {  2,  7,  3 }),

            new(GFX_COL.GFX_COL_GREY_2, new( 0x00,-0xA0, 0x00), new[] { 4,  3,  7, 11 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0xA0, 0x00, 0x00), new[] {  8,  5,  0,  4 }),
            new(GFX_COL.GFX_COL_GREY_2, new(-0xA0, 0x00, 0x00), new[] { 10,  7,  2,  6 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0x00, 0xA0, 0x00), new[] {  1,  5,  9,  6 }),

            new(GFX_COL.GFX_COL_GREY_3, new(-0x6B,-0x6B,-0x6B), new[] { 11,  7, 10 }),
            new(GFX_COL.GFX_COL_GREY_3, new( 0x6B,-0x6B,-0x6B), new[] { 11,  8,  4 }),
            new(GFX_COL.GFX_COL_GREY_3, new( 0x6B, 0x6B,-0x6B), new[] {  9,  5,  8 }),
            new(GFX_COL.GFX_COL_GREY_3, new(-0x6B, 0x6B,-0x6B), new[] { 10,  6,  9 }),

            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00,-0xA0), new[] { 11, 10,  9,  8 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x00, 0x00, 0xA0), new[] {  0,  1,  2,  3 }),

            new(GFX_COL.GFX_COL_BLACK,  new(0x00, 0x00, 0xA0), new[] { 15, 12, 13, 14 }),
        };


        public int LaserFront => 0;

        public int LaserStrength => 3;

        public ShipLine[] Lines { get; } =
                        {
            new(31,  1,  0,  0,  3),
            new(31,  2,  0,  0,  1),
            new(31,  3,  0,  1,  2),
            new(31,  4,  0,  2,  3),
            new(31,  5,  1,  3,  4),
            new(31,  6,  1,  0,  4),
            new(31,  6,  2,  0,  5),
            new(31,  8,  2,  5,  1),
            new(31,  8,  3,  1,  6),
            new(31,  7,  3,  2,  6),
            new(31,  7,  4,  2,  7),
            new(31,  5,  4,  3,  7),
            new(31, 13, 10,  8, 11),
            new(31, 13, 11,  8,  9),
            new(31, 13, 12,  9, 10),
            new(31, 13,  9, 10, 11),
            new(31, 10,  5,  4, 11),
            new(31, 10,  6,  4,  8),
            new(31, 11,  6,  5,  8),
            new(31, 11,  8,  5,  9),
            new(31, 12,  8,  6,  9),
            new(31, 12,  7,  6, 10),
            new(31,  9,  7,  7, 10),
            new(31,  9,  5,  7, 11),
            new(30,  0,  0, 12, 13),
            new(30,  0,  0, 13, 14),
            new(30,  0,  0, 14, 15),
            new(30,  0,  0, 15, 12),
        };
        public int LootMax => 0;

        public int MissilesMax => 6;

        public string Name => "Coriolis Space Station";

        public ShipPoint[] Points { get; } =
                                {
            new(new( 160,    0,  160), 31,  1,  0,  6,  2),
            new(new(   0,  160,  160), 31,  2,  0,  8,  3),
            new(new(-160,    0,  160), 31,  3,  0,  7,  4),
            new(new(   0, -160,  160), 31,  1,  0,  5,  4),
            new(new( 160, -160,    0), 31,  5,  1, 10,  6),
            new(new( 160,  160,    0), 31,  6,  2, 11,  8),
            new(new(-160,  160,    0), 31,  7,  3, 12,  8),
            new(new(-160, -160,    0), 31,  5,  4,  9,  7),
            new(new( 160,    0, -160), 31, 10,  6, 13, 11),
            new(new(   0,  160, -160), 31, 11,  8, 13, 12),
            new(new(-160,    0, -160), 31,  9,  7, 13, 12),
            new(new(   0, -160, -160), 31,  9,  5, 13, 10),
            new(new(  10,  -30,  160), 30,  0,  0,  0,  0),
            new(new(  10,   30,  160), 30,  0,  0,  0,  0),
            new(new( -10,   30,  160), 30,  0,  0,  0,  0),
            new(new( -10,  -30,  160), 30,  0,  0,  0,  0),
        };
        public StockType ScoopedType => StockType.None;
        public float Size => 25600;
        public ShipClass Type => ShipClass.Station;
        public int VanishPoint => 120;

        public float VelocityMax => 0;
    }
}
