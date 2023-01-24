namespace Elite.Engine.Ships
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static partial class shipdata
    {
        internal static readonly ship_point[] thargoid_point =
        {
            new(new(  32,  -48,   48), 31,  4,  0,  8,  8),
            new(new(  32,  -68,    0), 31,  1,  0,  4,  4),
            new(new(  32,  -48,  -48), 31,  2,  1,  4,  4),
            new(new(  32,    0,  -68), 31,  3,  2,  4,  4),
            new(new(  32,   48,  -48), 31,  4,  3,  5,  5),
            new(new(  32,   68,    0), 31,  5,  4,  6,  6),
            new(new(  32,   48,   48), 31,  6,  4,  7,  7),
            new(new(  32,    0,   68), 31,  7,  4,  8,  8),
            new(new( -24, -116,  116), 31,  8,  0,  9,  9),
            new(new( -24, -164,    0), 31,  1,  0,  9,  9),
            new(new( -24, -116, -116), 31,  2,  1,  9,  9),
            new(new( -24,    0, -164), 31,  3,  2,  9,  9),
            new(new( -24,  116, -116), 31,  5,  3,  9,  9),
            new(new( -24,  164,    0), 31,  6,  5,  9,  9),
            new(new( -24,  116,  116), 31,  7,  6,  9,  9),
            new(new( -24,    0,  164), 31,  8,  7,  9,  9),
            new(new( -24,   64,   80), 30,  9,  9,  9,  9),
            new(new( -24,   64,  -80), 30,  9,  9,  9,  9),
            new(new( -24,  -64,  -80), 30,  9,  9,  9,  9),
            new(new( -24,  -64,   80), 30,  9,  9,  9,  9),
        };

        internal static readonly ship_line[] thargoid_line =
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

        internal static readonly ship_face_normal[] thargoid_face_normal =
        {
            new(31, new( 103,  -60,   25)),
            new(31, new( 103,  -60,  -25)),
            new(31, new( 103,  -25,  -60)),
            new(31, new( 103,   25,  -60)),
            new(31, new(  64,    0,    0)),
            new(31, new( 103,   60,  -25)),
            new(31, new( 103,   60,   25)),
            new(31, new( 103,   25,   60)),
            new(31, new( 103,  -25,   60)),
            new(31, new( -48,    0,    0)),
        };

        private static readonly ship_face[] thargoid_face =
        {
            new(GFX_COL.GFX_COL_DARK_RED, new(   0x67,-0x3C, 0x19), new[] { 1,  0,  8,  9 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0x67,-0x3C,-0x19), new[] { 2,  1,  9, 10 }),
            new(GFX_COL.GFX_COL_DARK_RED, new(   0x67,-0x19,-0x3C), new[] { 3,  2, 10, 11 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0x67, 0x19,-0x3C), new[] { 4,  3, 11, 12 }),
		/*
			new(GFX_COL.GFX_COL_GREY_3,	 0x40, 0x00, 0x00, 8,  7,  6,  5,  4,  3,  2, 1, 0),
		*/
			new(GFX_COL.GFX_COL_GREY_3, new( 0x40, 0x00, 0x00), new[] { 0,  1,  2,  7 }),
            new(GFX_COL.GFX_COL_GREY_3, new( 0x40, 0x00, 0x00), new[] { 2,  3,  6,  7 }),
            new(GFX_COL.GFX_COL_GREY_3, new( 0x40, 0x00, 0x00), new[] { 3,  4,  5,  6 }),

            new(GFX_COL.GFX_COL_DARK_RED, new(   0x67, 0x3C,-0x19), new[] { 5,  4, 12, 13 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0x67, 0x3C, 0x19), new[] { 6,  5, 13, 14 }),
            new(GFX_COL.GFX_COL_DARK_RED, new(   0x67, 0x19, 0x3C), new[] { 7,  6, 14, 15 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0x67,-0x19, 0x3C), new[] { 0,  7, 15,  8 }),
		/*
			new(GFX_COL.GFX_COL_GREY_3,	-0x30, 0x00, 0x00, 8, 15, 14, 13, 12, 11, 10, 9, 8),
		*/
			new(GFX_COL.GFX_COL_GREY_3, new(-0x30, 0x00, 0x00), new[] { 9,  8, 15, 10 }),
            new(GFX_COL.GFX_COL_GREY_3, new(-0x30, 0x00, 0x00), new[] { 11, 10, 15, 14 }),
            new (GFX_COL.GFX_COL_GREY_3, new(-0x30, 0x00, 0x00), new[] { 12, 11, 14, 13 }),

            new(GFX_COL.GFX_COL_WHITE, new(     -0x30, 0x00, 0x00), new[] { 16, 17 /*, 19 */ }),
            new(GFX_COL.GFX_COL_WHITE, new(     -0x30, 0x00, 0x00), new[] { 18, 19 /*, 16 */ }),
        };

        internal static ship_data thargoid_data = new(
            "Thargoid",
            0,
            0,
            9801,
            15,
            50,
            55,
            240,
            39,
            6,
            11,
            thargoid_point,
            thargoid_line,
            thargoid_face_normal,
            thargoid_face
        );
    }
}