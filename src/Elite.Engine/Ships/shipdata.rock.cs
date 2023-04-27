namespace Elite.Engine.Ships
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static partial class shipdata
    {
        internal static readonly ship_point[] rock_point =
        {
            new(new( -24,  -25,   16), 31,  1,  2,  3,  3),
            new(new(   0,   12,  -10), 31,  0,  2,  3,  3),
            new(new(  11,   -6,    2), 31,  0,  1,  3,  3),
            new(new(  12,   42,    7), 31,  0,  1,  2,  2),
        };

        internal static readonly ship_line[] rock_line =
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        internal static readonly ship_face_normal[] rock_face_normal =
        {
            new ship_face_normal(18, new(  30,    0,    0)),
            new ship_face_normal(20, new(  22,   32,   -8)),
            new ship_face_normal( 0, new(   0,    2,    0)),
            new ship_face_normal( 0, new(  17,   23,   95)),
        };

        private static readonly ship_face[] rock_face =
        {
            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00, 0x00 ), new[] { 3, 2, 1 }),
            new(GFX_COL.GFX_COL_GREY_2, new(0x00, 0x00, 0x00 ), new[] { 0, 2, 3 }),
            new(GFX_COL.GFX_COL_GREY_3, new(0x00, 0x00, 0x00 ), new[] { 3, 1 , 0 }),
            new(GFX_COL.GFX_COL_GREY_4, new(0x00, 0x00, 0x00 ), new[] { 0, 1, 2 }),
        };

        internal static ship_data rock_data = new(
            "Rock",
            0,
            StockType.Minerals,
            256,
            0,
            0,
            8,
            20,
            10,
            0,
            0,
            rock_point,
            rock_line,
            rock_face_normal,
            rock_face
        );
    }
}