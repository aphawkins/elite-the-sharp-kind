namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] anacnda_point = new ship_point[]
        {
            new(   0,    7,  -58, 30,  0,  1,  5,  5),
            new( -43,  -13,  -37, 30,  0,  1,  2,  2),
            new( -26,  -47,   -3, 30,  0,  2,  3,  3),
            new(  26,  -47,   -3, 30,  0,  3,  4,  4),
            new(  43,  -13,  -37, 30,  0,  4,  5,  5),
            new(   0,   48,  -49, 30,  1,  5,  6,  6),
            new( -69,   15,  -15, 30,  1,  2,  7,  7),
            new( -43,  -39,   40, 31,  2,  3,  8,  8),
            new(  43,  -39,   40, 31,  3,  4,  9,  9),
            new(  69,   15,  -15, 30,  4,  5, 10, 10),
            new( -43,   53,  -23, 31, 15, 15, 15, 15),
            new( -69,   -1,   32, 31,  2,  7,  8,  8),
            new(   0,    0,  254, 31, 15, 15, 15, 15),
            new(  69,   -1,   32, 31,  4,  9, 10, 10),
            new(  43,   53,  -23, 31, 15, 15, 15, 15),
        };

        internal static ship_line[] anacnda_line = new ship_line[25]
        {
            new(30,  0,  1,  0,  1),
            new(30,  0,  2,  1,  2),
            new(30,  0,  3,  2,  3),
            new(30,  0,  4,  3,  4),
            new(30,  0,  5,  0,  4),
            new(29,  1,  5,  0,  5),
            new(29,  1,  2,  1,  6),
            new(29,  2,  3,  2,  7),
            new(29,  3,  4,  3,  8),
            new(29,  4,  5,  4,  9),
            new(30,  1,  6,  5, 10),
            new(30,  1,  7,  6, 10),
            new(30,  2,  7,  6, 11),
            new(30,  2,  8,  7, 11),
            new(31,  3,  8,  7, 12),
            new(31,  3,  9,  8, 12),
            new(30,  4,  9,  8, 13),
            new(30,  4, 10,  9, 13),
            new(30,  5, 10,  9, 14),
            new(30,  5,  6,  5, 14),
            new(30,  6, 11, 10, 14),
            new(31,  7, 11, 10, 12),
            new(31,  7,  8, 11, 12),
            new(31,  9, 10, 12, 13),
            new(31, 10, 11, 12, 14),
        };

        internal static ship_face_normal[] anacnda_face_normal = new ship_face_normal[12]
        {
            new(30,    0,  -51,  -49),
            new(30,  -51,   18,  -87),
            new(30,  -77,  -57,  -19),
            new(31,    0,  -90,   16),
            new(30,   77,  -57,  -19),
            new(30,   51,   18,  -87),
            new(30,    0,  111,  -20),
            new(31,  -97,   72,   24),
            new(31, -108,  -68,   34),
            new(31,  108,  -68,   34),
            new(31,   97,   72,   24),
            new(31,    0,   94,   18),
        };

        internal static ship_data anacnda_data = new(
            "Anaconda",
            15, 25, 12,
            7,
            0,
            10000,
            12,
            0,
            36,
            252,
            14,
            7,
            31,
            anacnda_point,
            anacnda_line,
            anacnda_face_normal);
    }
}