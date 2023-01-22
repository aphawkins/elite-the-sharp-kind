namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] constrct_point = new ship_point[]
        {
            new(new(  20,   -7,   80), 31,  0,  2,  9,  9),
            new(new( -20,   -7,   80), 31,  0,  1,  9,  9),
            new(new( -54,   -7,   40), 31,  1,  4,  9,  9),
            new(new( -54,   -7,  -40), 31,  4,  5,  8,  9),
            new(new( -20,   13,  -40), 31,  5,  6,  8,  8),
            new(new(  20,   13,  -40), 31,  6,  7,  8,  8),
            new(new(  54,   -7,  -40), 31,  3,  7,  8,  9),
            new(new(  54,   -7,   40), 31,  2,  3,  9,  9),
            new(new(  20,   13,    5), 31, 15, 15, 15, 15),
            new(new( -20,   13,    5), 31, 15, 15, 15, 15),
            new(new(  20,   -7,   62), 18,  9,  9,  9,  9),
            new(new( -20,   -7,   62), 18,  9,  9,  9,  9),
            new(new(  25,   -7,  -25), 18,  9,  9,  9,  9),
            new(new( -25,   -7,  -25), 18,  9,  9,  9,  9),
            new(new(  15,   -7,  -15), 10,  9,  9,  9,  9),
            new(new( -15,   -7,  -15), 10,  9,  9,  9,  9),
            new(new(   0,   -7,    0),  0,  9, 15,  0,  1),
        };

        internal static ship_line[] constrct_line = new ship_line[]
        {
            new(31,  0,  9,  0,  1),
            new(31,  1,  9,  1,  2),
            new(31,  0,  1,  1,  9),
            new(31,  0,  2,  0,  8),
            new(31,  2,  9,  0,  7),
            new(31,  2,  3,  7,  8),
            new(31,  1,  4,  2,  9),
            new(31,  4,  9,  2,  3),
            new(31,  3,  9,  6,  7),
            new(31,  3,  7,  6,  8),
            new(31,  6,  7,  5,  8),
            new(31,  5,  6,  4,  9),
            new(31,  4,  5,  3,  9),
            new(31,  5,  8,  3,  4),
            new(31,  6,  8,  4,  5),
            new(31,  7,  8,  5,  6),
            new(31,  8,  9,  3,  6),
            new(31,  0,  6,  8,  9),
            new(18,  9,  9, 10, 12),
            new( 5,  9,  9, 12, 14),
            new(10,  9,  9, 14, 10),
            new(10,  9,  9, 11, 15),
            new( 5,  9,  9, 13, 15),
            new(18,  9,  9, 11, 13),
        };

        internal static ship_face_normal[] constrct_face_normal = new ship_face_normal[10]
        {
            new(31, new(   0,   55,   15)),
            new(31, new( -24,   75,   20)),
            new(31, new(  24,   75,   20)),
            new(31, new(  44,   75,    0)),
            new(31, new( -44,   75,    0)),
            new(31, new( -44,   75,    0)),
            new(31, new(   0,   53,    0)),
            new(31, new(  44,   75,    0)),
            new(31, new(   0,    0, -160)),
            new(31, new(   0,  -27,    0)),
        };

        internal static ship_data constrct_data = new(
            "Constrictor",
            3,
            0,
            4225,
            0,
            0,
            45,
            252,
            36,
            4,
            26,
            constrct_point,
            constrct_line,
            constrct_face_normal);
    }
}