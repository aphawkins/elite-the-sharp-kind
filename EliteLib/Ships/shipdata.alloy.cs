namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static ship_point[] alloy_point = new ship_point[4]
        {
            new( -15,  -22,   -9, 31, 15, 15, 15, 15),
            new( -15,   38,   -9, 31, 15, 15, 15, 15),
            new(  19,   32,   11, 20, 15, 15, 15, 15),
            new(  10,  -46,    6, 20, 15, 15, 15, 15),
        };

        private static ship_line[] alloy_line = new ship_line[4]
        {
            new(31, 15, 15,  0,  1),
            new(16, 15, 15,  1,  2),
            new(20, 15, 15,  2,  3),
            new(16, 15, 15,  3,  0),
        };

        private static ship_face_normal[] alloy_face_normal = new ship_face_normal[1]
        {
            new ship_face_normal( 0,    0,    0,    0),
        };

        internal static ship_data alloy_data = new(
            "Alloy",
            4, 4, 1,
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
            alloy_face_normal
        );
    }
}