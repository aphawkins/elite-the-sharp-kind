namespace Elite.Ships
{
    using Elite.Enums;
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static readonly ship_point[] krait_point =
        {
            new(new(   0,    0,   96), 31,  0,  1,  2,  3),
            new(new(   0,   18,  -48), 31,  0,  3,  4,  5),
            new(new(   0,  -18,  -48), 31,  1,  2,  4,  5),
            new(new(  90,    0,   -3), 31,  0,  1,  4,  4),
            new(new( -90,    0,   -3), 31,  2,  3,  5,  5),
            new(new(  90,    0,   87), 30,  0,  1,  1,  1),
            new(new( -90,    0,   87), 30,  2,  3,  3,  3),
            new(new(   0,    5,   53),  9,  0,  0,  3,  3),
            new(new(   0,    7,   38),  6,  0,  0,  3,  3),
            new(new( -18,    7,   19),  9,  3,  3,  3,  3),
            new(new(  18,    7,   19),  9,  0,  0,  0,  0),
            new(new(  18,   11,  -39),  8,  4,  4,  4,  4),
            new(new(  18,  -11,  -39),  8,  4,  4,  4,  4),
            new(new(  36,    0,  -30),  8,  4,  4,  4,  4),
            new(new( -18,   11,  -39),  8,  5,  5,  5,  5),
            new(new( -18,  -11,  -39),  8,  5,  5,  5,  5),
            new(new( -36,    0,  -30),  8,  5,  5,  5,  5),
        };

        internal static readonly ship_line[] krait_line =
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
            new( 8,  4,  5,  1,  2),
            new( 9,  0,  0,  7, 10),
            new( 6,  0,  0,  8, 10),
            new( 9,  3,  3,  7,  9),
            new( 6,  3,  3,  8,  9),
            new( 8,  4,  4, 11, 13),
            new( 8,  4,  4, 13, 12),
            new( 7,  4,  4, 12, 11),
            new( 7,  5,  5, 14, 15),
            new( 8,  5,  5, 15, 16),
            new( 8,  5,  5, 16, 14),
        };

        internal static readonly ship_face_normal[] krait_face_normal =
        {
            new(31, new(   3,   24,    3)),
            new(31, new(   3,  -24,    3)),
            new(31, new(  -3,  -24,    3)),
            new(31, new(  -3,   24,    3)),
            new(31, new(  38,    0,  -77)),
            new(31, new( -38,    0,  -77)),
        };

        private static readonly ship_face[] krait_face =
        {
            new(GFX_COL.GFX_COL_BLUE_3, new( 0x03, 0x18, 0x03), 3,  0,  3,  1, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_BLUE_2, new( 0x03,-0x18, 0x03), 3,  2,  3,  0, 0, 0, 0, 0, 0),

            new(GFX_COL.GFX_COL_BLUE_3, new(-0x03,-0x18, 0x03), 3,  0,  4,  2, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_BLUE_2, new(-0x03, 0x18, 0x03), 3,  1,  4,  0, 0, 0, 0, 0, 0),

            new(GFX_COL.GFX_COL_GREY_3, new( 0x26, 0x00,-0x4D), 3,  3,  2,  1, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_GREY_1, new(-0x26, 0x00,-0x4D), 3,  4,  1,  2, 0, 0, 0, 0, 0),

            new(GFX_COL.GFX_COL_WHITE, new( 0x03,-0x18, 0x03), 2,  3,  5,  0, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_WHITE, new( 0x03, 0x18, 0x03), 2,  5,  3,  0, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_WHITE, new(-0x03, 0x18, 0x03), 2,  4,  6,  0, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_WHITE, new(-0x03,-0x18, 0x03), 2,  6,  4,  0, 0, 0, 0, 0, 0),

            new(GFX_COL.GFX_COL_RED, new(     0x26, 0x00,-0x4D), 3, 12, 11, 13, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_RED, new(    -0x26, 0x00,-0x4D), 3, 16, 14, 15, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_WHITE, new(   0x03, 0x18, 0x03), 3,  7, 10,  8, 0, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_WHITE, new(  -0x03, 0x18, 0x03), 3,  8,  9,  7, 0, 0, 0, 0, 0),
        };

        internal static ship_data krait_data = new(
            "Krait",
            1,
            0,
            3600,
            0,
            10.0f,
            20,
            80,
            30,
            0,
            8,
            krait_point,
            krait_line,
            krait_face_normal,
            krait_face
        );
    }
}