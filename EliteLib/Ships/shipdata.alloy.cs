namespace Elite.Ships
{
    using Elite.Enums;
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static readonly ship_point[] alloy_point =
        {
            new(new( -15,  -22,   -9), 31, 15, 15, 15, 15),
            new(new( -15,   38,   -9), 31, 15, 15, 15, 15),
            new(new(  19,   32,   11), 20, 15, 15, 15, 15),
            new(new(  10,  -46,    6), 20, 15, 15, 15, 15),
        };

        private static readonly ship_line[] alloy_line =
        {
            new(31, 15, 15,  0,  1),
            new(16, 15, 15,  1,  2),
            new(20, 15, 15,  2,  3),
            new(16, 15, 15,  3,  0),
        };

        private static readonly ship_face_normal[] alloy_face_normal =
        {
            new ship_face_normal(0, new(0, 0, 0)),
        };

        private static readonly ship_face[] alloy_face =
        {
            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00, 0x00), 4, 0, 1, 2, 3, 0, 0, 0, 0),
            new(GFX_COL.GFX_COL_GREY_3, new(0x00, 0x00, 0x00), 4, 3, 2, 1, 0, 0, 0, 0, 0),
        };

        internal static ship_data alloy_data = new(
            "Alloy",
            0,
            8,
            100,
            0,
            0,
            5,
            16,
            16,
            0,
            0,
            alloy_point,
            alloy_line,
            alloy_face_normal,
            alloy_face
        );
    }
}