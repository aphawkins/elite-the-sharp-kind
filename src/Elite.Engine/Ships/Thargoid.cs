// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Thargoid : IShip
    {
        public float Bounty => 50;

        public int EnergyMax => 240;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(103,  -60,   25)),
            new(31, new(103,  -60,  -25)),
            new(31, new(103,  -25,  -60)),
            new(31, new(103,   25,  -60)),
            new(31, new(64,    0,    0)),
            new(31, new(103,   60,  -25)),
            new(31, new(103,   60,   25)),
            new(31, new(103,   25,   60)),
            new(31, new(103,  -25,   60)),
            new(31, new(-48,    0,    0)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.LightRed, new(0x67, -0x3C, 0x19), new[] { 1,  0,  8,  9 }),
            new(Colour.DarkGrey, new(0x67, -0x3C, -0x19), new[] { 2,  1,  9, 10 }),
            new(Colour.LightRed, new(0x67, -0x19, -0x3C), new[] { 3,  2, 10, 11 }),
            new(Colour.DarkGrey, new(0x67, 0x19, -0x3C), new[] { 4,  3, 11, 12 }),

            //new(graphics_COL.graphics_COL_GREY_3,    0x40, 0x00, 0x00, 8,  7,  6,  5,  4,  3,  2, 1, 0),
            new(Colour.DarkerGrey, new(0x40, 0x00, 0x00), new[] { 0,  1,  2,  7 }),
            new(Colour.DarkerGrey, new(0x40, 0x00, 0x00), new[] { 2,  3,  6,  7 }),
            new(Colour.DarkerGrey, new(0x40, 0x00, 0x00), new[] { 3,  4,  5,  6 }),

            new(Colour.LightRed, new(0x67, 0x3C, -0x19), new[] { 5,  4, 12, 13 }),
            new(Colour.DarkGrey, new(0x67, 0x3C, 0x19), new[] { 6,  5, 13, 14 }),
            new(Colour.LightRed, new(0x67, 0x19, 0x3C), new[] { 7,  6, 14, 15 }),
            new(Colour.DarkGrey, new(0x67, -0x19, 0x3C), new[] { 0,  7, 15,  8 }),

            //new(graphics_COL.graphics_COL_GREY_3,   -0x30, 0x00, 0x00, 8, 15, 14, 13, 12, 11, 10, 9, 8),
            new(Colour.DarkerGrey, new(-0x30, 0x00, 0x00), new[] { 9,  8, 15, 10 }),
            new(Colour.DarkerGrey, new(-0x30, 0x00, 0x00), new[] { 11, 10, 15, 14 }),
            new(Colour.DarkerGrey, new(-0x30, 0x00, 0x00), new[] { 12, 11, 14, 13 }),

            new(Colour.White, new(-0x30, 0x00, 0x00), new[] { 16, 17 /*, 19 */ }),
            new(Colour.White, new(-0x30, 0x00, 0x00), new[] { 18, 19 /*, 16 */ }),
        };

        public int LaserFront => 15;

        public int LaserStrength => 11;

        public ShipLine[] Lines { get; } =
        {
            new(31,  8,  4,  0,  7),
            new(31,  4,  0,  0,  1),
            new(31,  4,  1,  1,  2),
            new(31,  4,  2,  2,  3),
            new(31,  4,  3,  3,  4),
            new(31,  5,  4,  4,  5),
            new(31,  6,  4,  5,  6),
            new(31,  7,  4,  6,  7),
            new(31,  8,  0,  0,  8),
            new(31,  1,  0,  1,  9),
            new(31,  2,  1,  2, 10),
            new(31,  3,  2,  3, 11),
            new(31,  5,  3,  4, 12),
            new(31,  6,  5,  5, 13),
            new(31,  7,  6,  6, 14),
            new(31,  8,  7,  7, 15),
            new(31,  9,  8,  8, 15),
            new(31,  9,  0,  8,  9),
            new(31,  9,  1,  9, 10),
            new(31,  9,  2, 10, 11),
            new(31,  9,  3, 11, 12),
            new(31,  9,  5, 12, 13),
            new(31,  9,  6, 13, 14),
            new(31,  9,  7, 14, 15),
            new(30,  9,  9, 16, 17),
            new(30,  9,  9, 18, 19),
        };

        public int LootMax => 0;

        public int MissilesMax => 6;

        public string Name => "Thargoid";

        public ShipPoint[] Points { get; } =
        {
            new(new(32,  -48,   48), 31,  4,  0,  8,  8),
            new(new(32,  -68,    0), 31,  1,  0,  4,  4),
            new(new(32,  -48,  -48), 31,  2,  1,  4,  4),
            new(new(32,    0,  -68), 31,  3,  2,  4,  4),
            new(new(32,   48,  -48), 31,  4,  3,  5,  5),
            new(new(32,   68,    0), 31,  5,  4,  6,  6),
            new(new(32,   48,   48), 31,  6,  4,  7,  7),
            new(new(32,    0,   68), 31,  7,  4,  8,  8),
            new(new(-24, -116,  116), 31,  8,  0,  9,  9),
            new(new(-24, -164,    0), 31,  1,  0,  9,  9),
            new(new(-24, -116, -116), 31,  2,  1,  9,  9),
            new(new(-24,    0, -164), 31,  3,  2,  9,  9),
            new(new(-24,  116, -116), 31,  5,  3,  9,  9),
            new(new(-24,  164,    0), 31,  6,  5,  9,  9),
            new(new(-24,  116,  116), 31,  7,  6,  9,  9),
            new(new(-24,    0,  164), 31,  8,  7,  9,  9),
            new(new(-24,   64,   80), 30,  9,  9,  9,  9),
            new(new(-24,   64,  -80), 30,  9,  9,  9,  9),
            new(new(-24,  -64,  -80), 30,  9,  9,  9,  9),
            new(new(-24,  -64,   80), 30,  9,  9,  9,  9),
        };

        public StockType ScoopedType => StockType.None;

        public float Size => 9801;

        public ShipClass Type => ShipClass.LoneWolf;

        public int VanishPoint => 55;

        public float VelocityMax => 39;
    }
}
