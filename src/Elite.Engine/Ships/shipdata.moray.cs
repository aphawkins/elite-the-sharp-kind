using Elite.Engine.Enums;
using Elite.Engine.Types;

namespace Elite.Engine.Ships
{
    internal static partial class Ship
    {
        internal static readonly ShipPoint[] moray_point =
        {
            new(new(  15,    0,   65), 31,  0,  2,  7,  8),
            new(new( -15,    0,   65), 31,  0,  1,  6,  7),
            new(new(   0,   18,  -40), 17, 15, 15, 15, 15),
            new(new( -60,    0,    0), 31,  1,  3,  6,  6),
            new(new(  60,    0,    0), 31,  2,  5,  8,  8),
            new(new(  30,  -27,  -10), 24,  4,  5,  7,  8),
            new(new( -30,  -27,  -10), 24,  3,  4,  6,  7),
            new(new(  -9,   -4,  -25),  7,  4,  4,  4,  4),
            new(new(   9,   -4,  -25),  7,  4,  4,  4,  4),
            new(new(   0,  -18,  -16),  7,  4,  4,  4,  4),
            new(new(  13,    3,   49),  5,  0,  0,  0,  0),
            new(new(   6,    0,   65),  5,  0,  0,  0,  0),
            new(new( -13,    3,   49),  5,  0,  0,  0,  0),
            new(new(  -6,    0,   65),  5,  0,  0,  0,  0),
        };

        internal static readonly ShipLine[] moray_line =
        {
            new(31,  0,  7,  0,  1),
            new(31,  1,  6,  1,  3),
            new(24,  3,  6,  3,  6),
            new(24,  4,  7,  5,  6),
            new(24,  5,  8,  4,  5),
            new(31,  2,  8,  0,  4),
            new(15,  6,  7,  1,  6),
            new(15,  7,  8,  0,  5),
            new(15,  0,  2,  0,  2),
            new(15,  0,  1,  1,  2),
            new(17,  1,  3,  2,  3),
            new(17,  2,  5,  2,  4),
            new(13,  4,  5,  2,  5),
            new(13,  3,  4,  2,  6),
            new( 5,  4,  4,  7,  8),
            new( 7,  4,  4,  7,  9),
            new( 7,  4,  4,  8,  9),
            new( 5,  0,  0, 10, 11),
            new( 5,  0,  0, 12, 13),
        };

        internal static readonly ShipFaceNormal[] moray_face_normal =
        {
            new(31, new(   0,   43,    7)),
            new(31, new( -10,   49,    7)),
            new(31, new(  10,   49,    7)),
            new(24, new( -59,  -28, -101)),
            new(24, new(   0,  -52,  -78)),
            new(24, new(  59,  -28, -101)),
            new(31, new( -72,  -99,   50)),
            new(31, new(   0,  -83,   30)),
            new(31, new(  72,  -99,   50)),
        };

        private static readonly ShipFace[] moray_face =
        {
            new(GFX_COL.GFX_COL_BLUE_4, new( 0x00, 0x2B, 0x07), new[] {   0,  2, 1 }),
            new(GFX_COL.GFX_COL_BLUE_3, new(-0x0A, 0x31, 0x07), new[] {   1,  2, 3 }),
            new (GFX_COL.GFX_COL_BLUE_3, new( 0x0A, 0x31, 0x07), new[] {  4,  2, 0 }),

            new(GFX_COL.GFX_COL_GREY_1, new(-0x3B,-0x1C,-0x65), new[] { 3,  2, 6 }),
            new (GFX_COL.GFX_COL_GREY_3, new( 0x00,-0x34,-0x4E), new[] { 6,  2, 5 }),
            new (GFX_COL.GFX_COL_GREY_1, new( 0x3B,-0x1C,-0x65), new[] { 5,  2, 4 }),

            new (GFX_COL.GFX_COL_BLUE_1, new(-0x48,-0x63, 0x32), new[] { 6,  1, 3 }),
            new(GFX_COL.GFX_COL_BLUE_2, new( 0x00,-0x53, 0x1E), new[] { 6,  5, 0, 1 }),
            new(GFX_COL.GFX_COL_BLUE_1, new( 0x48,-0x63, 0x32), new[] { 4,  0, 5 }),

            new(GFX_COL.GFX_COL_DARK_RED, new(0x00,-0x34,-0x4E), new[] { 8,  9, 7 }),

            new(GFX_COL.GFX_COL_WHITE, new( 0x00, 0x2B, 0x07), new[] { 11, 10 /*, 12 */ }),
            new(GFX_COL.GFX_COL_WHITE, new( 0x00, 0x2B, 0x07), new[] { 12, 13 /*, 10 */ }),
        };

        internal static ShipData moray_data = new(
            "Moray Star Boat",
            1,
            0,
            900,
            0,
            5,
            40,
            100,
            25,
            0,
            8,
            moray_point,
            moray_line,
            moray_face_normal,
            moray_face
        );
    }
}