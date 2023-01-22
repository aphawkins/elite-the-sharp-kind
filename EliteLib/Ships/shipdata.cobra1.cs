namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] cobra1_point = new ship_point[]
        {
            new( -18,   -1,   50, 31,  0,  1,  2,  3),
            new(  18,   -1,   50, 31,  0,  1,  4,  5),
            new( -66,    0,    7, 31,  2,  3,  8,  8),
            new(  66,    0,    7, 31,  4,  5,  9,  9),
            new( -32,   12,  -38, 31,  2,  6,  7,  8),
            new(  32,   12,  -38, 31,  4,  6,  7,  9),
            new( -54,  -12,  -38, 31,  1,  3,  7,  8),
            new(  54,  -12,  -38, 31,  1,  5,  7,  9),
            new(   0,   12,   -6, 20,  0,  2,  4,  6),
            new(   0,   -1,   50,  2,  0,  1,  1,  1),
            new(   0,   -1,   60, 31,  0,  1,  1,  1),
        };

        internal static ship_line[] cobra1_line = new ship_line[]
        {
            new(31,  0,  1,  1,  0),
            new(31,  2,  3,  0,  2),
            new(31,  3,  8,  2,  6),
            new(31,  1,  7,  6,  7),
            new(31,  5,  9,  7,  3),
            new(31,  4,  5,  3,  1),
            new(31,  2,  8,  2,  4),
            new(31,  6,  7,  4,  5),
            new(31,  4,  9,  5,  3),
            new(20,  0,  2,  0,  8),
            new(20,  0,  4,  8,  1),
            new(16,  2,  6,  4,  8),
            new(16,  4,  6,  8,  5),
            new(31,  7,  8,  4,  6),
            new(31,  7,  9,  5,  7),
            new(20,  1,  3,  0,  6),
            new(20,  1,  5,  1,  7),
            new( 2,  0,  1, 10,  9),
        };

        internal static ship_face_normal[] cobra1_face_normal = new ship_face_normal[10]
        {
            new(31,    0,   41,   10),
            new(31,    0,  -27,    3),
            new(31,   -8,   46,    8),
            new(31,  -12,  -57,   12),
            new(31,    8,   46,    8),
            new(31,   12,  -57,   12),
            new(31,    0,   49,    0),
            new(31,    0,    0, -154),
            new(31, -121,  111,  -62),
            new(31,  121,  111,  -62),
        };

        internal static ship_data cobra1_data = new(
            "Cobra MkI",
            11, 18, 10,
            3,
            0,
            9801,
            10,
            7.5f,
            19,
            90,
            26,
            2,
            9,
            cobra1_point,
            cobra1_line,
            cobra1_face_normal);
    }
}