namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] krait_point = new ship_point[]
        {
            new(   0,    0,   96, 31,  0,  1,  2,  3),
            new(   0,   18,  -48, 31,  0,  3,  4,  5),
            new(   0,  -18,  -48, 31,  1,  2,  4,  5),
            new(  90,    0,   -3, 31,  0,  1,  4,  4),
            new( -90,    0,   -3, 31,  2,  3,  5,  5),
            new(  90,    0,   87, 30,  0,  1,  1,  1),
            new( -90,    0,   87, 30,  2,  3,  3,  3),
            new(   0,    5,   53,  9,  0,  0,  3,  3),
            new(   0,    7,   38,  6,  0,  0,  3,  3),
            new( -18,    7,   19,  9,  3,  3,  3,  3),
            new(  18,    7,   19,  9,  0,  0,  0,  0),
            new(  18,   11,  -39,  8,  4,  4,  4,  4),
            new(  18,  -11,  -39,  8,  4,  4,  4,  4),
            new(  36,    0,  -30,  8,  4,  4,  4,  4),
            new( -18,   11,  -39,  8,  5,  5,  5,  5),
            new( -18,  -11,  -39,  8,  5,  5,  5,  5),
            new( -36,    0,  -30,  8,  5,  5,  5,  5),
        };

        internal static ship_line[] krait_line = new ship_line[]
        {
            new(31,  0,  3,  0,  1),
            new(31,  1,  2,  0,  2),
            new(31,  0,  1,  0,  3),
            new(31,  2,  3,  0,  4),
            new(31,  3,  5,  1,  4),
            new(31,  2,  5,  4,  2),
            new(31,  1,  4,  2,  3),
            new(31,  0,  4,  3,  1),
            new(30,  0,  1,  3,  5),
            new(30,  2,  3,  4,  6),
            new( 8,  4,  5,  1,  2),
            new( 9,  0,  0,  7, 10),
            new( 6,  0,  0,  8, 10),
            new( 9,  3,  3,  7,  9),
            new( 6,  3,  3,  8,  9),
            new( 8,  4,  4, 11, 13),
            new( 8,  4,  4, 13, 12),
            new( 7,  4,  4, 12, 11),
            new( 7,  5,  5, 14, 15),
            new( 8,  5,  5, 15, 16),
            new( 8,  5,  5, 16, 14),
        };

        internal static ship_face_normal[] krait_face_normal = new ship_face_normal[6]
        {
            new(31,    3,   24,    3),
            new(31,    3,  -24,    3),
            new(31,   -3,  -24,    3),
            new(31,   -3,   24,    3),
            new(31,   38,    0,  -77),
            new(31,  -38,    0,  -77),
        };

        internal static ship_data krait_data = new(
            "Krait",
            17, 21, 6,
            1,
            0,
            3600,
            0,
            10.0f,
            20,
            80,
            30,
            0,
            8,
            krait_point,
            krait_line,
            krait_face_normal);
    }
}