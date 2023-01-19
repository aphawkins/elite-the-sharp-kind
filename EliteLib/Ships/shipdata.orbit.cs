namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] orbit_point = new ship_point[]
        {
            new(   0,  -17,   23, 31, 15, 15, 15, 15),
            new( -17,    0,   23, 31, 15, 15, 15, 15),
            new(   0,   18,   23, 31, 15, 15, 15, 15),
            new(  18,    0,   23, 31, 15, 15, 15, 15),
            new( -20,  -20,  -27, 31,  1,  2,  3,  9),
            new( -20,   20,  -27, 31,  3,  4,  5,  9),
            new(  20,   20,  -27, 31,  5,  6,  7,  9),
            new(  20,  -20,  -27, 31,  1,  7,  8,  9),
            new(   5,    0,  -27, 16,  9,  9,  9,  9),
            new(   0,   -2,  -27, 16,  9,  9,  9,  9),
            new(  -5,    0,  -27,  9,  9,  9,  9,  9),
            new(   0,    3,  -27,  9,  9,  9,  9,  9),
            new(   0,   -9,   35, 16,  0, 10, 11, 12),
            new(   3,   -1,   31,  7, 15, 15,  0,  2),
            new(   4,   11,   25,  8,  0,  1, 15,  4),
            new(  11,    4,   25,  8, 10,  1,  3, 15),
            new(  -3,   -1,   31,  7,  6, 11,  2,  3),
            new(  -3,   11,   25,  8, 15,  8, 12,  0),
            new( -10,    4,   25,  8,  4, 15,  1,  8),
        };

        internal static ship_line[] orbit_line = new ship_line[30]
        {
            new(31,  0,  2,  0,  1),
            new(31,  4, 10,  1,  2),
            new(31,  6, 11,  2,  3),
            new(31,  8, 12,  0,  3),
            new(31,  1,  8,  0,  7),
            new(24,  1,  2,  0,  4),
            new(31,  2,  3,  1,  4),
            new(24,  3,  4,  1,  5),
            new(31,  4,  5,  2,  5),
            new(12,  5,  6,  2,  6),
            new(31,  6,  7,  3,  6),
            new(24,  7,  8,  3,  7),
            new(31,  3,  9,  4,  5),
            new(31,  5,  9,  5,  6),
            new(31,  7,  9,  6,  7),
            new(31,  1,  9,  4,  7),
            new(16,  0, 12,  0, 12),
            new(16,  0, 10,  1, 12),
            new(16, 10, 11,  2, 12),
            new(16, 11, 12,  3, 12),
            new(16,  9,  9,  8,  9),
            new( 7,  9,  9,  9, 10),
            new( 9,  9,  9, 10, 11),
            new( 7,  9,  9,  8, 11),
            new( 5, 11, 11, 13, 14),
            new( 8, 11, 11, 14, 15),
            new( 7, 11, 11, 13, 15),
            new( 5, 10, 10, 16, 17),
            new( 8, 10, 10, 17, 18),
            new( 7, 10, 10, 16, 18),
        };

        internal static ship_face_normal[] orbit_face_normal = new ship_face_normal[13]
        {
            new ship_face_normal(31,  -55,  -55,   40),
            new ship_face_normal(31,    0,  -74,    4),
            new ship_face_normal(31,  -51,  -51,   23),
            new ship_face_normal(31,  -74,    0,    4),
            new ship_face_normal(31,  -51,   51,   23),
            new ship_face_normal(31,    0,   74,    4),
            new ship_face_normal(31,   51,   51,   23),
            new ship_face_normal(31,   74,    0,    4),
            new ship_face_normal(31,   51,  -51,   23),
            new ship_face_normal(31,    0,    0, -107),
            new ship_face_normal(31,  -41,   41,   90),
            new ship_face_normal(31,   41,   41,   90),
            new ship_face_normal(31,   55,  -55,   40),
        };

        internal static ship_data orbit_data = new(
            "Orbit Shuttle",
            19, 30, 13,
            15,
            0,
            2500,
            0,
            0,
            22,
            32,
            8,
            0,
            0,
            orbit_point,
            orbit_line,
            orbit_face_normal
        );
    }
}