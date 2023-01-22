namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static ship_point[] boulder_point = new ship_point[]
        {
            new( -18,   37,  -11, 31,  0,  1,  5,  9),
            new(  30,    7,   12, 31,  1,  2,  5,  6),
            new(  28,   -7,  -12, 31,  2,  3,  6,  7),
            new(   2,    0,  -39, 31,  3,  4,  7,  8),
            new( -28,   34,  -30, 31,  0,  4,  8,  9),
            new(   5,  -10,   13, 31, 15, 15, 15, 15),
            new(  20,   17,  -30, 31, 15, 15, 15, 15),
        };

        private static ship_line[] boulder_line = new ship_line[]
        {
            new(31,  1,  5,  0,  1),
            new(31,  2,  6,  1,  2),
            new(31,  3,  7,  2,  3),
            new(31,  4,  8,  3,  4),
            new(31,  0,  9,  4,  0),
            new(31,  0,  1,  0,  5),
            new(31,  1,  2,  1,  5),
            new(31,  2,  3,  2,  5),
            new(31,  3,  4,  3,  5),
            new(31,  0,  4,  4,  5),
            new(31,  5,  9,  0,  6),
            new(31,  5,  6,  1,  6),
            new(31,  6,  7,  2,  6),
            new(31,  7,  8,  3,  6),
            new(31,  8,  9,  4,  6),
        };

        private static ship_face_normal[] boulder_face_normal = new ship_face_normal[10]
        {
            new ship_face_normal(31,  -15,   -3,    8),
            new ship_face_normal(31,   -7,   12,   30),
            new ship_face_normal(31,   32,  -47,   24),
            new ship_face_normal(31,   -3,  -39,   -7),
            new ship_face_normal(31,   -5,   -4,   -1),
            new ship_face_normal(31,   49,   84,    8),
            new ship_face_normal(31,  112,   21,  -21),
            new ship_face_normal(31,   76,  -35,  -82),
            new ship_face_normal(31,   22,   56, -137),
            new ship_face_normal(31,   40,  110,  -38),
        };

        internal static ship_data boulder_data = new(
            "Boulder",
            7, 15, 10,
            0,
            0,
            900,
            0,
            0.1f,
            20,
            20,
            30,
            0,
            0,
            boulder_point,
            boulder_line,
            boulder_face_normal
        );
    } 
}