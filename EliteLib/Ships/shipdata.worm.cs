namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] worm_point = new ship_point[]
        {
            new(  10,  -10,   35, 31,  0,  2,  7,  7),
            new( -10,  -10,   35, 31,  0,  3,  7,  7),
            new(   5,    6,   15, 31,  0,  1,  2,  4),
            new(  -5,    6,   15, 31,  0,  1,  3,  5),
            new(  15,  -10,   25, 31,  2,  4,  7,  7),
            new( -15,  -10,   25, 31,  3,  5,  7,  7),
            new(  26,  -10,  -25, 31,  4,  6,  7,  7),
            new( -26,  -10,  -25, 31,  5,  6,  7,  7),
            new(   8,   14,  -25, 31,  1,  4,  6,  6),
            new(  -8,   14,  -25, 31,  1,  5,  6,  6),
        };

        internal static ship_line[] worm_line = new ship_line[]
        {
            new(31,  0,  7,  0,  1),
            new(31,  3,  7,  1,  5),
            new(31,  5,  7,  5,  7),
            new(31,  6,  7,  7,  6),
            new(31,  4,  7,  6,  4),
            new(31,  2,  7,  4,  0),
            new(31,  0,  2,  0,  2),
            new(31,  0,  3,  1,  3),
            new(31,  2,  4,  4,  2),
            new(31,  3,  5,  5,  3),
            new(31,  1,  4,  2,  8),
            new(31,  4,  6,  8,  6),
            new(31,  1,  5,  3,  9),
            new(31,  5,  6,  9,  7),
            new(31,  0,  1,  2,  3),
            new(31,  1,  6,  8,  9),
        };

        internal static ship_face_normal[] worm_face_normal = new ship_face_normal[8]
        {
            new(31,    0,   88,   70),
            new(31,    0,   69,   14),
            new(31,   70,   66,   35),
            new(31,  -70,   66,   35),
            new(31,   64,   49,   14),
            new(31,  -64,   49,   14),
            new(31,    0,    0, -200),
            new(31,    0,  -80,    0),
        };

        internal static ship_data worm_data = new(
            "Worm",
            0,
            0,
            9801,
            0,
            0,
            19,
            30,
            23,
            0,
            4,
            worm_point,
            worm_line,
            worm_face_normal);
    }
}