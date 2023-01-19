namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] ferdlce_point = new ship_point[]
        {
            new(   0,  -14,  108, 31,  0,  1,  5,  9),
            new( -40,  -14,   -4, 31,  1,  2,  9,  9),
            new( -12,  -14,  -52, 31,  2,  3,  9,  9),
            new(  12,  -14,  -52, 31,  3,  4,  9,  9),
            new(  40,  -14,   -4, 31,  4,  5,  9,  9),
            new( -40,   14,   -4, 28,  0,  1,  2,  6),
            new( -12,    2,  -52, 28,  2,  3,  6,  7),
            new(  12,    2,  -52, 28,  3,  4,  7,  8),
            new(  40,   14,   -4, 28,  0,  4,  5,  8),
            new(   0,   18,  -20, 15,  0,  6,  7,  8),
            new(  -3,  -11,   97, 11,  0,  0,  0,  0),
            new( -26,    8,   18,  9,  0,  0,  0,  0),
            new( -16,   14,   -4, 11,  0,  0,  0,  0),
            new(   3,  -11,   97, 11,  0,  0,  0,  0),
            new(  26,    8,   18,  9,  0,  0,  0,  0),
            new(  16,   14,   -4, 11,  0,  0,  0,  0),
            new(   0,  -14,  -20, 12,  9,  9,  9,  9),
            new( -14,  -14,   44, 12,  9,  9,  9,  9),
            new(  14,  -14,   44, 12,  9,  9,  9,  9),
        };

        internal static ship_line[] ferdlce_line = new ship_line[27]
        {
            new(31,  1,  9,  0,  1),
            new(31,  2,  9,  1,  2),
            new(31,  3,  9,  2,  3),
            new(31,  4,  9,  3,  4),
            new(31,  5,  9,  0,  4),
            new(28,  0,  1,  0,  5),
            new(28,  2,  6,  5,  6),
            new(28,  3,  7,  6,  7),
            new(28,  4,  8,  7,  8),
            new(28,  0,  5,  0,  8),
            new(15,  0,  6,  5,  9),
            new(11,  6,  7,  6,  9),
            new(11,  7,  8,  7,  9),
            new(15,  0,  8,  8,  9),
            new(14,  1,  2,  1,  5),
            new(14,  2,  3,  2,  6),
            new(14,  3,  4,  3,  7),
            new(14,  4,  5,  4,  8),
            new( 8,  0,  0, 10, 11),
            new( 9,  0,  0, 11, 12),
            new(11,  0,  0, 10, 12),
            new( 8,  0,  0, 13, 14),
            new( 9,  0,  0, 14, 15),
            new(11,  0,  0, 13, 15),
            new(12,  9,  9, 16, 17),
            new(12,  9,  9, 16, 18),
            new( 8,  9,  9, 17, 18),
        };

        internal static ship_face_normal[] ferdlce_face_normal = new ship_face_normal[10]
        {
            new(28,    0,   24,    6),
            new(31,  -68,    0,   24),
            new(31,  -63,    0,  -37),
            new(31,    0,    0, -104),
            new(31,   63,    0,  -37),
            new(31,   68,    0,   24),
            new(28,  -12,   46,  -19),
            new(28,    0,   45,  -22),
            new(28,   12,   46,  -19),
            new(31,    0,  -28,    0),
        };

        internal static ship_data ferdlce_data = new(
            "Fer-de-Lance",
            19, 27, 10,
            0,
            0,
            1600,
            0,
            0,
            40,
            160,
            30,
            2,
            9,
            ferdlce_point,
            ferdlce_line,
            ferdlce_face_normal);
    }
}