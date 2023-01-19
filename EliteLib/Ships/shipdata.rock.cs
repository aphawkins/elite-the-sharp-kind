namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] rock_point = new ship_point[]
        {
            new( -24,  -25,   16, 31,  1,  2,  3,  3),
            new(   0,   12,  -10, 31,  0,  2,  3,  3),
            new(  11,   -6,    2, 31,  0,  1,  3,  3),
            new(  12,   42,    7, 31,  0,  1,  2,  2),
        };

        internal static ship_line[] rock_line = new ship_line[]
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        internal static ship_face_normal[] rock_face_normal = new ship_face_normal[4]
        {
            new ship_face_normal(18,   30,    0,    0),
            new ship_face_normal(20,   22,   32,   -8),
            new ship_face_normal( 0,    0,    2,    0),
            new ship_face_normal( 0,   17,   23,   95),
        };

        internal static ship_data rock_data = new(
            "Rock",
            4, 6, 4,
            0,
            11,
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
            rock_face_normal
        );
    }
}