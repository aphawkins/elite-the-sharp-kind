namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] moray_point = new ship_point[]
        {
            new(  15,    0,   65, 31,  0,  2,  7,  8),
            new( -15,    0,   65, 31,  0,  1,  6,  7),
            new(   0,   18,  -40, 17, 15, 15, 15, 15),
            new( -60,    0,    0, 31,  1,  3,  6,  6),
            new(  60,    0,    0, 31,  2,  5,  8,  8),
            new(  30,  -27,  -10, 24,  4,  5,  7,  8),
            new( -30,  -27,  -10, 24,  3,  4,  6,  7),
            new(  -9,   -4,  -25,  7,  4,  4,  4,  4),
            new(   9,   -4,  -25,  7,  4,  4,  4,  4),
            new(   0,  -18,  -16,  7,  4,  4,  4,  4),
            new(  13,    3,   49,  5,  0,  0,  0,  0),
            new(   6,    0,   65,  5,  0,  0,  0,  0),
            new( -13,    3,   49,  5,  0,  0,  0,  0),
            new(  -6,    0,   65,  5,  0,  0,  0,  0),
        };

        internal static ship_line[] moray_line = new ship_line[19]
        {
            new(31,  0,  7,  0,  1),
            new(31,  1,  6,  1,  3),
            new(24,  3,  6,  3,  6),
            new(24,  4,  7,  5,  6),
            new(24,  5,  8,  4,  5),
            new(31,  2,  8,  0,  4),
            new(15,  6,  7,  1,  6),
            new(15,  7,  8,  0,  5),
            new(15,  0,  2,  0,  2),
            new(15,  0,  1,  1,  2),
            new(17,  1,  3,  2,  3),
            new(17,  2,  5,  2,  4),
            new(13,  4,  5,  2,  5),
            new(13,  3,  4,  2,  6),
            new( 5,  4,  4,  7,  8),
            new( 7,  4,  4,  7,  9),
            new( 7,  4,  4,  8,  9),
            new( 5,  0,  0, 10, 11),
            new( 5,  0,  0, 12, 13),
        };

        internal static ship_face_normal[] moray_face_normal = new ship_face_normal[9]
        {
            new(31,    0,   43,    7),
            new(31,  -10,   49,    7),
            new(31,   10,   49,    7),
            new(24,  -59,  -28, -101),
            new(24,    0,  -52,  -78),
            new(24,   59,  -28, -101),
            new(31,  -72,  -99,   50),
            new(31,    0,  -83,   30),
            new(31,   72,  -99,   50),
        };

        internal static ship_data moray_data = new(
            "Moray Star Boat",
            14, 19, 9,
            1,
            0,
            900,
            0,
            50,
            40,
            100,
            25,
            0,
            8,
            moray_point,
            moray_line,
            moray_face_normal);
    }
}