namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] gecko_point = new ship_point[]
        {
            new( -10,   -4,   47, 31,  0,  3,  4,  5),
            new(  10,   -4,   47, 31,  0,  1,  2,  3),
            new( -16,    8,  -23, 31,  0,  5,  6,  7),
            new(  16,    8,  -23, 31,  0,  1,  7,  8),
            new( -66,    0,   -3, 31,  4,  5,  6,  6),
            new(  66,    0,   -3, 31,  1,  2,  8,  8),
            new( -20,  -14,  -23, 31,  3,  4,  6,  7),
            new(  20,  -14,  -23, 31,  2,  3,  7,  8),
            new(  -8,   -6,   33, 16,  3,  3,  3,  3),
            new(   8,   -6,   33, 17,  3,  3,  3,  3),
            new(  -8,  -13,  -16, 16,  3,  3,  3,  3),
            new(   8,  -13,  -16, 17,  3,  3,  3,  3),
        };

        internal static ship_line[] gecko_line = new ship_line[]
        {
            new(31,  0,  3,  0,  1),
            new(31,  1,  2,  1,  5),
            new(31,  1,  8,  5,  3),
            new(31,  0,  7,  3,  2),
            new(31,  5,  6,  2,  4),
            new(31,  4,  5,  4,  0),
            new(31,  2,  8,  5,  7),
            new(31,  3,  7,  7,  6),
            new(31,  4,  6,  6,  4),
            new(29,  0,  5,  0,  2),
            new(30,  0,  1,  1,  3),
            new(29,  3,  4,  0,  6),
            new(30,  2,  3,  1,  7),
            new(20,  6,  7,  2,  6),
            new(20,  7,  8,  3,  7),
            new(16,  3,  3,  8, 10),
            new(17,  3,  3,  9, 11),
        };

        internal static ship_face_normal[] gecko_face_normal = new ship_face_normal[9]
        {
            new(31,    0,   31,    5),
            new(31,    4,   45,    8),
            new(31,   25, -108,   19),
            new(31,    0,  -84,   12),
            new(31,  -25, -108,   19),
            new(31,   -4,   45,    8),
            new(31,  -88,   16, -214),
            new(31,    0,    0, -187),
            new(31,   88,   16, -214),
        };

        internal static ship_data gecko_data = new(
            "Gecko",
            12, 17, 9,
            0,
            0,
            9801,
            0,
            55,
            18,
            70,
            30,
            0,
            8,
            gecko_point,
            gecko_line,
            gecko_face_normal);
    }
}