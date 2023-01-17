namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] transp_point = new ship_point[37]
        {
            new(   0,   10,  -26, 31,  0,  6,  7,  7),
            new( -25,    4,  -26, 31,  0,  1,  7,  7),
            new( -28,   -3,  -26, 31,  0,  1,  2,  2),
            new( -25,   -8,  -26, 31,  0,  2,  3,  3),
            new(  26,   -8,  -26, 31,  0,  3,  4,  4),
            new(  29,   -3,  -26, 31,  0,  4,  5,  5),
            new(  26,    4,  -26, 31,  0,  5,  6,  6),
            new(   0,    6,   12, 19, 15, 15, 15, 15),
            new( -30,   -1,   12, 31,  1,  7,  8,  9),
            new( -33,   -8,   12, 31,  1,  2,  3,  9),
            new(  33,   -8,   12, 31,  3,  4,  5, 10),
            new(  30,   -1,   12, 31,  5,  6, 10, 11),
            new( -11,   -2,   30, 31,  8,  9, 12, 13),
            new( -13,   -8,   30, 31,  3,  9, 13, 13),
            new(  14,   -8,   30, 31,  3, 10, 13, 13),
            new(  11,   -2,   30, 31, 10, 11, 12, 13),
            new(  -5,    6,    2,  7,  7,  7,  7,  7),
            new( -18,    3,    2,  7,  7,  7,  7,  7),
            new(  -5,    7,   -7,  7,  7,  7,  7,  7),
            new( -18,    4,   -7,  7,  7,  7,  7,  7),
            new( -11,    6,  -14,  7,  7,  7,  7,  7),
            new( -11,    5,   -7,  7,  7,  7,  7,  7),
            new(   5,    7,  -14,  7,  6,  6,  6,  6),
            new(  18,    4,  -14,  7,  6,  6,  6,  6),
            new(  11,    5,   -7,  7,  6,  6,  6,  6),
            new(   5,    6,   -3,  7,  6,  6,  6,  6),
            new(  18,    3,   -3,  7,  6,  6,  6,  6),
            new(  11,    4,    8,  7,  6,  6,  6,  6),
            new(  11,    5,   -3,  7,  6,  6,  6,  6),
            new( -16,   -8,  -13,  6,  3,  3,  3,  3),
            new( -16,   -8,   16,  6,  3,  3,  3,  3),
            new(  17,   -8,  -13,  6,  3,  3,  3,  3),
            new(  17,   -8,   16,  6,  3,  3,  3,  3),
            new( -13,   -3,  -26,  8,  0,  0,  0,  0),
            new(  13,   -3,  -26,  8,  0,  0,  0,  0),
            new(   9,    3,  -26,  5,  0,  0,  0,  0),
            new(  -8,    3,  -26,  5,  0,  0,  0,  0),
        };

        internal static ship_line[] transp_line = new ship_line[46]
        {
            new(31,  0,  7,  0,  1),
            new(31,  0,  1,  1,  2),
            new(31,  0,  2,  2,  3),
            new(31,  0,  3,  3,  4),
            new(31,  0,  4,  4,  5),
            new(31,  0,  5,  5,  6),
            new(31,  0,  6,  0,  6),
            new(16,  6,  7,  0,  7),
            new(31,  1,  7,  1,  8),
            new(11,  1,  2,  2,  9),
            new(31,  2,  3,  3,  9),
            new(31,  3,  4,  4, 10),
            new(11,  4,  5,  5, 10),
            new(31,  5,  6,  6, 11),
            new(17,  7,  8,  7,  8),
            new(17,  1,  9,  8,  9),
            new(17,  5, 10, 10, 11),
            new(17,  6, 11,  7, 11),
            new(19, 11, 12,  7, 15),
            new(19,  8, 12,  7, 12),
            new(16,  8,  9,  8, 12),
            new(31,  3,  9,  9, 13),
            new(31,  3, 10, 10, 14),
            new(16, 10, 11, 11, 15),
            new(31,  9, 13, 12, 13),
            new(31,  3, 13, 13, 14),
            new(31, 10, 13, 14, 15),
            new(31, 12, 13, 12, 15),
            new( 7,  7,  7, 16, 17),
            new( 7,  7,  7, 18, 19),
            new( 7,  7,  7, 19, 20),
            new( 7,  7,  7, 18, 20),
            new( 7,  7,  7, 20, 21),
            new( 7,  6,  6, 22, 23),
            new( 7,  6,  6, 23, 24),
            new( 7,  6,  6, 24, 22),
            new( 7,  6,  6, 25, 26),
            new( 7,  6,  6, 26, 27),
            new( 7,  6,  6, 25, 27),
            new( 7,  6,  6, 27, 28),
            new( 6,  3,  3, 29, 30),
            new( 6,  3,  3, 31, 32),
            new( 8,  0,  0, 33, 34),
            new( 5,  0,  0, 34, 35),
            new( 5,  0,  0, 35, 36),
            new( 5,  0,  0, 36, 33),
        };

        internal static ship_face_normal[] transp_face_normal = new ship_face_normal[14]
        {
            new ship_face_normal(31,    0,    0, -103),
            new ship_face_normal(31, -111,   48,   -7),
            new ship_face_normal(31, -105,  -63,  -21),
            new ship_face_normal(31,    0,  -34,    0),
            new ship_face_normal(31,  105,  -63,  -21),
            new ship_face_normal(31,  111,   48,   -7),
            new ship_face_normal(31,    8,   32,    3),
            new ship_face_normal(31,   -8,   32,    3),
            new ship_face_normal(19,   -8,   34,   11),
            new ship_face_normal(31,  -75,   32,   79),
            new ship_face_normal(31,   75,   32,   79),
            new ship_face_normal(19,    8,   34,   11),
            new ship_face_normal(31,    0,   38,   17),
            new ship_face_normal(31,    0,    0,  121),
        };

        internal static ship_data transp_data = new(
            "Transporter",
            37, 46, 14,
            0,
            0,
            2500,
            12,
            0,
            16,
            32,
            10,
            0,
            0,
            transp_point,
            transp_line,
            transp_face_normal
        );
    }
}