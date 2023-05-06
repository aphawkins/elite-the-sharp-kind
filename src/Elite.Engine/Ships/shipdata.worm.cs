using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal static partial class Ship
    {
        internal static readonly ShipPoint[] worm_point =
        {
            new(new(  10,  -10,   35), 31,  0,  2,  7,  7),
            new(new( -10,  -10,   35), 31,  0,  3,  7,  7),
            new(new(   5,    6,   15), 31,  0,  1,  2,  4),
            new(new(  -5,    6,   15), 31,  0,  1,  3,  5),
            new(new(  15,  -10,   25), 31,  2,  4,  7,  7),
            new(new( -15,  -10,   25), 31,  3,  5,  7,  7),
            new(new(  26,  -10,  -25), 31,  4,  6,  7,  7),
            new(new( -26,  -10,  -25), 31,  5,  6,  7,  7),
            new(new(   8,   14,  -25), 31,  1,  4,  6,  6),
            new(new(  -8,   14,  -25), 31,  1,  5,  6,  6),
        };

        internal static readonly ShipLine[] worm_line =
        {
            new(31,  0,  7,  0,  1),
            new(31,  3,  7,  1,  5),
            new(31,  5,  7,  5,  7),
            new(31,  6,  7,  7,  6),
            new(31,  4,  7,  6,  4),
            new(31,  2,  7,  4,  0),
            new(31,  0,  2,  0,  2),
            new(31,  0,  3,  1,  3),
            new(31,  2,  4,  4,  2),
            new(31,  3,  5,  5,  3),
            new(31,  1,  4,  2,  8),
            new(31,  4,  6,  8,  6),
            new(31,  1,  5,  3,  9),
            new(31,  5,  6,  9,  7),
            new(31,  0,  1,  2,  3),
            new(31,  1,  6,  8,  9),
        };

        internal static readonly ShipFaceNormal[] worm_face_normal =
        {
            new(31, new(   0,   88,   70)),
            new(31, new(   0,   69,   14)),
            new(31, new(  70,   66,   35)),
            new(31, new( -70,   66,   35)),
            new(31, new(  64,   49,   14)),
            new(31, new( -64,   49,   14)),
            new(31, new(   0,    0, -200)),
            new(31, new(   0,  -80,    0)),
        };

        private static readonly ShipFace[] worm_face =
        {
            new(GFX_COL.GFX_COL_GREY_4, new( 0x00, 0x58, 0x46), new[] { 1, 0, 2, 3 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x46, 0x42, 0x23), new[] { 0, 4, 2 }),
            new(GFX_COL.GFX_COL_GREY_1, new(-0x46, 0x42, 0x23), new[] { 1, 3, 5 }),

            new(GFX_COL.GFX_COL_GREY_2, new( 0x40, 0x31, 0x0E), new[] { 2, 4, 6, 8 }),
            new(GFX_COL.GFX_COL_GREY_2, new(-0x40, 0x31, 0x0E), new[] { 5, 3, 9, 7 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x00, 0x00,-0xC8), new[] { 6, 7, 9, 8 }),

            new(GFX_COL.GFX_COL_GREY_3, new( 0x00,-0x50, 0x00), new[] { 4, 0, 1, 5, 7, 6 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x00, 0x45, 0x0E), new[] { 9, 3, 2, 8 }),
        };

        internal static ShipData worm_data = new(
            "Worm",
            0,
            0,
            9801,
            0,
            0,
            19,
            30,
            23,
            0,
            4,
            worm_point,
            worm_line,
            worm_face_normal,
            worm_face
        );
    }
}