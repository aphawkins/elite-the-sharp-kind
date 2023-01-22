namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] boa_point = new ship_point[]
        {
            new(new(   0,    0,   93), 31, 15, 15, 15, 15),
            new(new(   0,   40,  -87), 24,  0,  2,  3,  3),
            new(new(  38,  -25,  -99), 24,  0,  1,  4,  4),
            new(new( -38,  -25,  -99), 24,  1,  2,  5,  5),
            new(new( -38,   40,  -59), 31,  2,  3,  6,  9),
            new(new(  38,   40,  -59), 31,  0,  3,  6, 11),
            new(new(  62,    0,  -67), 31,  0,  4,  8, 11),
            new(new(  24,  -65,  -79), 31,  1,  4,  8, 10),
            new(new( -24,  -65,  -79), 31,  1,  5,  7, 10),
            new(new( -62,    0,  -67), 31,  2,  5,  7,  9),
            new(new(   0,    7, -107), 22,  0,  2, 10, 10),
            new(new(  13,   -9, -107), 22,  0,  1, 10, 10),
            new(new( -13,   -9, -107), 22,  1,  2, 12, 12),
        };

        internal static ship_line[] boa_line = new ship_line[]
        {
            new(31,  6, 11,  0,  5),
            new(31,  8, 10,  0,  7),
            new(31,  7,  9,  0,  9),
            new(29,  6,  9,  0,  4),
            new(29,  8, 11,  0,  6),
            new(29,  7, 10,  0,  8),
            new(31,  3,  6,  4,  5),
            new(31,  0, 11,  5,  6),
            new(31,  4,  8,  6,  7),
            new(31,  1, 10,  7,  8),
            new(31,  5,  7,  8,  9),
            new(31,  2,  9,  4,  9),
            new(24,  2,  3,  1,  4),
            new(24,  0,  3,  1,  5),
            new(24,  2,  5,  3,  9),
            new(24,  1,  5,  3,  8),
            new(24,  0,  4,  2,  6),
            new(24,  1,  4,  2,  7),
            new(22,  0,  2,  1, 10),
            new(22,  0,  1,  2, 11),
            new(22,  1,  2,  3, 12),
            new(14,  0, 12, 10, 11),
            new(14,  1, 12, 11, 12),
            new(14,  2, 12, 12, 10),
        };

        internal static ship_face_normal[] boa_face_normal = new ship_face_normal[13]
        {
            new(31, new(  43,   37,  -60)),
            new(31, new(   0,  -45,  -89)),
            new(31, new( -43,   37,  -60)),
            new(31, new(   0,   40,    0)),
            new(31, new(  62,  -32,  -20)),
            new(31, new( -62,  -32,  -20)),
            new(31, new(   0,   23,    6)),
            new(31, new( -23,  -15,    9)),
            new(31, new(  23,  -15,    9)),
            new(31, new( -26,   13,   10)),
            new(31, new(   0,  -31,   12)),
            new(31, new(  26,   13,   10)),
            new(14, new(   0,    0, -107)),
        };

        internal static ship_data boa_data = new(
            "Boa",
            5,
            0,
            4900,
            0,
            0,
            40,
            250,
            24,
            4,
            14,
            boa_point,
            boa_line,
            boa_face_normal);
    }
}