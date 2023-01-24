namespace Elite.Engine.Ships
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static partial class shipdata
    {
        internal static readonly ship_point[] cobra3b_point =
        {
            new(new(  32,    0,   76), 31, 15, 15, 15, 15),
            new(new( -32,    0,   76), 31, 15, 15, 15, 15),
            new(new(   0,   26,   24), 31, 15, 15, 15, 15),
            new(new(-120,   -3,   -8), 31,  7,  3, 10, 10),
            new(new( 120,   -3,   -8), 31,  8,  4, 12, 12),
            new(new( -88,   16,  -40), 31, 15, 15, 15, 15),
            new(new(  88,   16,  -40), 31, 15, 15, 15, 15),
            new(new( 128,   -8,  -40), 31,  9,  8, 12, 12),
            new(new(-128,   -8,  -40), 31,  9,  7, 10, 10),
            new(new(   0,   26,  -40), 31,  6,  5,  9,  9),
            new(new( -32,  -24,  -40), 31, 10,  9, 11, 11),
            new(new(  32,  -24,  -40), 31, 11,  9, 12, 12),
            new(new( -36,    8,  -40), 20,  9,  9,  9,  9),
            new(new(  -8,   12,  -40), 20,  9,  9,  9,  9),
            new(new(   8,   12,  -40), 20,  9,  9,  9,  9),
            new(new(  36,    8,  -40), 20,  9,  9,  9,  9),
            new(new(  36,  -12,  -40), 20,  9,  9,  9,  9),
            new(new(   8,  -16,  -40), 20,  9,  9,  9,  9),
            new(new(  -8,  -16,  -40), 20,  9,  9,  9,  9),
            new(new( -36,  -12,  -40), 20,  9,  9,  9,  9),
            new(new(   0,    0,   76),  6, 11,  0, 11, 11),
            new(new(   0,    0,   90), 31, 11,  0, 11, 11),
            new(new( -80,   -6,  -40),  8,  9,  9,  9,  9),
            new(new( -80,    6,  -40),  8,  9,  9,  9,  9),
            new(new( -88,    0,  -40),  6,  9,  9,  9,  9),
            new(new(  80,    6,  -40),  8,  9,  9,  9,  9),
            new(new(  88,    0,  -40),  6,  9,  9,  9,  9),
            new(new(  80,   -6,  -40),  8,  9,  9,  9,  9),
        };

        internal static readonly ship_line[] cobra3b_line =
        {
            new(31, 11,  0,  0,  1),
            new(31, 12,  4,  0,  4),
            new(31, 10,  3,  1,  3),
            new(31, 10,  7,  3,  8),
            new(31, 12,  8,  4,  7),
            new(31,  9,  8,  6,  7),
            new(31,  9,  6,  6,  9),
            new(31,  9,  5,  5,  9),
            new(31,  9,  7,  5,  8),
            new(31,  5,  1,  2,  5),
            new(31,  6,  2,  2,  6),
            new(31,  7,  3,  3,  5),
            new(31,  8,  4,  4,  6),
            new(31,  1,  0,  1,  2),
            new(31,  2,  0,  0,  2),
            new(31, 10,  9,  8, 10),
            new(31, 11,  9, 10, 11),
            new(31, 12,  9,  7, 11),
            new(31, 11, 10,  1, 10),
            new(31, 12, 11,  0, 11),
            new(29,  3,  1,  1,  5),
            new(29,  4,  2,  0,  6),
            new( 6, 11,  0, 20, 21),
            new(20,  9,  9, 12, 13),
            new(20,  9,  9, 18, 19),
            new(20,  9,  9, 14, 15),
            new(20,  9,  9, 16, 17),
            new(19,  9,  9, 15, 16),
            new(17,  9,  9, 14, 17),
            new(19,  9,  9, 13, 18),
            new(19,  9,  9, 12, 19),
            new(30,  6,  5,  2,  9),
            new( 6,  9,  9, 22, 24),
            new( 6,  9,  9, 23, 24),
            new( 8,  9,  9, 22, 23),
            new( 6,  9,  9, 25, 26),
            new( 6,  9,  9, 26, 27),
            new( 8,  9,  9, 25, 27),
        };

        internal static readonly ship_face_normal[] cobra3b_face_normal =
        {
            new(31, new(   0,   62,   31)),
            new(31, new( -18,   55,   16)),
            new(31, new(  18,   55,   16)),
            new(31, new( -16,   52,   14)),
            new(31, new(  16,   52,   14)),
            new(31, new( -14,   47,    0)),
            new(31, new(  14,   47,    0)),
            new(31, new( -61,  102,    0)),
            new(31, new(  61,  102,    0)),
            new(31, new(   0,    0,  -80)),
            new(31, new(  -7,  -42,    9)),
            new(31, new(   0,  -30,    6)),
            new(31, new(   7,  -42,    9)),
        };

        private static readonly ship_face[] cobra3b_face =
{
            new(GFX_COL.GFX_COL_GREY_2, new(0x00, 0x3E, 0x1F), new[] {  1,  0,  2 }),
            new(GFX_COL.GFX_COL_BLUE_1, new(  -0x12, 0x37, 0x10), new[] {  5,  1,  2 }),
            new(GFX_COL.GFX_COL_BLUE_1, new(0x12, 0x37, 0x10), new[] {  2,  0,  6 }),
            new(GFX_COL.GFX_COL_BLUE_3, new(  -0x10, 0x34, 0x0E), new[] {  3,  1,  5 }),
            new(GFX_COL.GFX_COL_BLUE_3, new(0x10, 0x34, 0x0E), new[] {  6,  0,  4 }),

            new(GFX_COL.GFX_COL_GREY_1, new(  -0x0E, 0x2F, 0x00), new[] {  5,  2,  9 }),
            new(GFX_COL.GFX_COL_GREY_1, new(0x0E, 0x2F, 0x00), new[] {  9,  2,  6 }),

            new(GFX_COL.GFX_COL_BLUE_2, new(  -0x3D, 0x66, 0x00), new[] {  8,  3,  5 }),
            new(GFX_COL.GFX_COL_BLUE_2, new(0x3D, 0x66, 0x00), new[] {  6,  4,  7 }),

            new(GFX_COL.GFX_COL_GREY_2, new(0x00, 0x00,-0x50), new[] {  6,  7, 11, 10, 8, 5, 9 }),

            new(GFX_COL.GFX_COL_GREY_3, new(  -0x07,-0x2A, 0x09), new[] { 10,  1,  3,  8 }),
            new(GFX_COL.GFX_COL_DARK_RED, new(  0x00,-0x1E, 0x06), new[] { 10, 11,  0,  1 }),
            new(GFX_COL.GFX_COL_GREY_3, new(0x07,-0x2A, 0x09), new[] {  7,  4,  0, 11 }),

            new(GFX_COL.GFX_COL_RED, new(       0x00, 0x00,-0x50), new[] { 17, 14, 15, 16 }),
            new(GFX_COL.GFX_COL_RED, new(       0x00, 0x00,-0x50), new[] { 19, 12, 13, 18 }),
            new(GFX_COL.GFX_COL_DARK_RED, new( 0x00, 0x00,-0x50), new[] { 23, 22, 24 }),
            new(GFX_COL.GFX_COL_DARK_RED, new( 0x00, 0x00,-0x50), new[] { 27, 25, 26 }),

            new(GFX_COL.GFX_COL_WHITE, new(     0x00, 0x3E, 0x1F), new[] {  20, 21 }),
            new(GFX_COL.GFX_COL_WHITE, new(     0x00,-0x1E, 0x06), new[] {  21, 20 }),
        };

        internal static ship_data cobra3b_data = new(
            "Cobra MkIII",
            1,
            0,
            9025,
            21,
            17.5f,
            50,
            150,
            28,
            2,
            9,
            cobra3b_point,
            cobra3b_line,
            cobra3b_face_normal,
            cobra3b_face
        );
    }
}