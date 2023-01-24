namespace Elite.Engine.Ships
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static partial class shipdata
    {
        private static readonly ship_point[] cargo_point =
        {
            new(new(  24,   16,    0), 31,  1,  0,  5,  5),
            new(new(  24,    5,   15), 31,  1,  0,  2,  2),
            new(new(  24,  -13,    9), 31,  2,  0,  3,  3),
            new(new(  24,  -13,   -9), 31,  3,  0,  4,  4),
            new(new(  24,    5,  -15), 31,  4,  0,  5,  5),
            new(new( -24,   16,    0), 31,  5,  1,  6,  6),
            new(new( -24,    5,   15), 31,  2,  1,  6,  6),
            new(new( -24,  -13,    9), 31,  3,  2,  6,  6),
            new(new( -24,  -13,   -9), 31,  4,  3,  6,  6),
            new(new( -24,    5,  -15), 31,  5,  4,  6,  6),
        };

        private static readonly ship_line[] cargo_line =
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

        private static readonly ship_face_normal[] cargo_face_normal =
        {
            new ship_face_normal(31, new(  96,    0,    0)),
            new ship_face_normal(31, new(   0,   41,   30)),
            new ship_face_normal(31, new(   0,  -18,   48)),
            new ship_face_normal(31, new(   0,  -51,    0)),
            new ship_face_normal(31, new(   0,  -18,  -48)),
            new ship_face_normal(31, new(   0,   41,  -30)),
            new ship_face_normal(31, new( -96,    0,    0)),
        };

        private static readonly ship_face[] cargo_face =
        {
            new(GFX_COL.GFX_COL_GREY_4, new( 0x60, 0x00, 0x00), new [] { 4, 0, 1, 2, 3 }),

            new(GFX_COL.GFX_COL_GREY_2, new(  0x00, 0x29, 0x1E), new [] {  5, 6, 1, 0 }),
            new (GFX_COL.GFX_COL_GREY_1, new(  0x00,-0x12, 0x30), new [] {  6, 7, 2, 1 }),
            new (GFX_COL.GFX_COL_GREY_3, new(  0x00,-0x33, 0x00), new [] {  7, 8, 3, 2 }),
            new (GFX_COL.GFX_COL_GREY_1, new(  0x00,-0x12,-0x30), new[] { 8, 9, 4, 3 }),
            new (GFX_COL.GFX_COL_GREY_3, new(  0x00, 0x29,-0x1E), new[] { 9, 5, 0, 4 }),

            new (GFX_COL.GFX_COL_GREY_4, new( -0x60, 0x00, 0x00), new[] { 8, 7, 6, 5, 9 }),
        };

        internal static ship_data cargo_data = new(
            "Cargo Canister",
            0,
            0,
            400,
            0,
            0,
            12,
            17,
            15,
            0,
            0,
            cargo_point,
            cargo_line,
            cargo_face_normal,
            cargo_face
        );
    }
}