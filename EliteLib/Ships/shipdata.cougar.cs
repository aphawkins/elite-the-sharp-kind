namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] cougar_point = new ship_point[]
        {
            new(   0,    5,   67, 31,  0,  2,  4,  4),
            new( -20,    0,   40, 31,  0,  1,  2,  2),
            new( -40,    0,  -40, 31,  0,  1,  5,  5),
            new(   0,   14,  -40, 30,  0,  4,  5,  5),
            new(   0,  -14,  -40, 30,  1,  2,  3,  5),
            new(  20,    0,   40, 31,  2,  3,  4,  4),
            new(  40,    0,  -40, 31,  3,  4,  5,  5),
            new( -36,    0,   56, 31,  0,  1,  1,  1),
            new( -60,    0,  -20, 31,  0,  1,  1,  1),
            new(  36,    0,   56, 31,  3,  4,  4,  4),
            new(  60,    0,  -20, 31,  3,  4,  4,  4),
            new(   0,    7,   35, 18,  0,  0,  4,  4),
            new(   0,    8,   25, 20,  0,  0,  4,  4),
            new( -12,    2,   45, 20,  0,  0,  0,  0),
            new(  12,    2,   45, 20,  4,  4,  4,  4),
            new( -10,    6,  -40, 20,  5,  5,  5,  5),
            new( -10,   -6,  -40, 20,  5,  5,  5,  5),
            new(  10,   -6,  -40, 20,  5,  5,  5,  5),
            new(  10,    6,  -40, 20,  5,  5,  5,  5),
        };

        internal static ship_line[] cougar_line = new ship_line[]
        {
            new(31,  0,  2,  0,  1),
            new(31,  0,  1,  1,  7),
            new(31,  0,  1,  7,  8),
            new(31,  0,  1,  8,  2),
            new(30,  0,  5,  2,  3),
            new(30,  4,  5,  3,  6),
            new(30,  1,  5,  2,  4),
            new(30,  3,  5,  4,  6),
            new(31,  3,  4,  6, 10),
            new(31,  3,  4, 10,  9),
            new(31,  3,  4,  9,  5),
            new(31,  2,  4,  5,  0),
            new(27,  0,  4,  0,  3),
            new(27,  1,  2,  1,  4),
            new(27,  2,  3,  5,  4),
            new(26,  0,  1,  1,  2),
            new(26,  3,  4,  5,  6),
            new(20,  0,  0, 12, 13),
            new(18,  0,  0, 13, 11),
            new(18,  4,  4, 11, 14),
            new(20,  4,  4, 14, 12),
            new(18,  5,  5, 15, 16),
            new(20,  5,  5, 16, 18),
            new(18,  5,  5, 18, 17),
            new(20,  5,  5, 17, 15),
        };

        internal static ship_face_normal[] cougar_face_normal = new ship_face_normal[6]
        {
            new ship_face_normal(31,  -16,   46,    4),
            new ship_face_normal(31,  -16,  -46,    4),
            new ship_face_normal(31,    0,  -27,    5),
            new ship_face_normal(31,   16,  -46,    4),
            new ship_face_normal(31,   16,   46,    4),
            new ship_face_normal(30,    0,    0, -160),
            };

        internal static ship_data cougar_data = new(
            "Cougar",
            3,
            0,
            4900,
            0,
            0,
            34,
            252,
            40,
            4,
            26,
            cougar_point,
            cougar_line,
            cougar_face_normal
        );
    }
}