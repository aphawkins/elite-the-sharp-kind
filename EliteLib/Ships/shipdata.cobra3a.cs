namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] cobra3a_point = new ship_point[]
        {
            new(new(  32,    0,   76), 31, 15, 15, 15, 15),
            new(new( -32,    0,   76), 31, 15, 15, 15, 15),
            new(new(   0,   26,   24), 31, 15, 15, 15, 15),
            new(new(-120,   -3,   -8), 31,  7,  3, 10, 10),
            new(new( 120,   -3,   -8), 31,  8,  4, 12, 12),
            new(new( -88,   16,  -40), 31, 15, 15, 15, 15),
            new(new(  88,   16,  -40), 31, 15, 15, 15, 15),
            new(new( 128,   -8,  -40), 31,  9,  8, 12, 12),
            new(new(-128,   -8,  -40), 31,  9,  7, 10, 10),
            new(new(   0,   26,  -40), 31,  6,  5,  9,  9),
            new(new( -32,  -24,  -40), 31, 10,  9, 11, 11),
            new(new(  32,  -24,  -40), 31, 11,  9, 12, 12),
            new(new( -36,    8,  -40), 20,  9,  9,  9,  9),
            new(new(  -8,   12,  -40), 20,  9,  9,  9,  9),
            new(new(   8,   12,  -40), 20,  9,  9,  9,  9),
            new(new(  36,    8,  -40), 20,  9,  9,  9,  9),
            new(new(  36,  -12,  -40), 20,  9,  9,  9,  9),
            new(new(   8,  -16,  -40), 20,  9,  9,  9,  9),
            new(new(  -8,  -16,  -40), 20,  9,  9,  9,  9),
            new(new( -36,  -12,  -40), 20,  9,  9,  9,  9),
            new(new(   0,    0,   76),  6, 11,  0, 11, 11),
            new(new(   0,    0,   90), 31, 11,  0, 11, 11),
            new(new( -80,   -6,  -40),  8,  9,  9,  9,  9),
            new(new( -80,    6,  -40),  8,  9,  9,  9,  9),
            new(new( -88,    0,  -40),  6,  9,  9,  9,  9),
            new(new(  80,    6,  -40),  8,  9,  9,  9,  9),
            new(new(  88,    0,  -40),  6,  9,  9,  9,  9),
            new(new(  80,   -6,  -40),  8,  9,  9,  9,  9),
        };

        internal static ship_line[] cobra3a_line = new ship_line[]
        {
            new(31, 11,  0,  0,  1),
            new(31, 12,  4,  0,  4),
            new(31, 10,  3,  1,  3),
            new(31, 10,  7,  3,  8),
            new(31, 12,  8,  4,  7),
            new(31,  9,  8,  6,  7),
            new(31,  9,  6,  6,  9),
            new(31,  9,  5,  5,  9),
            new(31,  9,  7,  5,  8),
            new(31,  5,  1,  2,  5),
            new(31,  6,  2,  2,  6),
            new(31,  7,  3,  3,  5),
            new(31,  8,  4,  4,  6),
            new(31,  1,  0,  1,  2),
            new(31,  2,  0,  0,  2),
            new(31, 10,  9,  8, 10),
            new(31, 11,  9, 10, 11),
            new(31, 12,  9,  7, 11),
            new(31, 11, 10,  1, 10),
            new(31, 12, 11,  0, 11),
            new(29,  3,  1,  1,  5),
            new(29,  4,  2,  0,  6),
            new( 6, 11,  0, 20, 21),
            new(20,  9,  9, 12, 13),
            new(20,  9,  9, 18, 19),
            new(20,  9,  9, 14, 15),
            new(20,  9,  9, 16, 17),
            new(19,  9,  9, 15, 16),
            new(17,  9,  9, 14, 17),
            new(19,  9,  9, 13, 18),
            new(19,  9,  9, 12, 19),
            new(30,  6,  5,  2,  9),
            new( 6,  9,  9, 22, 24),
            new( 6,  9,  9, 23, 24),
            new( 8,  9,  9, 22, 23),
            new( 6,  9,  9, 25, 26),
            new( 6,  9,  9, 26, 27),
            new( 8,  9,  9, 25, 27),
        };

        internal static ship_face_normal[] cobra3a_face_normal = new ship_face_normal[13]
        {
            new ship_face_normal(31, new(   0,   62,   31)),
            new ship_face_normal(31, new( -18,   55,   16)),
            new ship_face_normal(31, new(  18,   55,   16)),
            new ship_face_normal(31, new( -16,   52,   14)),
            new ship_face_normal(31, new(  16,   52,   14)),
            new ship_face_normal(31, new( -14,   47,    0)),
            new ship_face_normal(31, new(  14,   47,    0)),
            new ship_face_normal(31, new( -61,  102,    0)),
            new ship_face_normal(31, new(  61,  102,    0)),
            new ship_face_normal(31, new(   0,    0,  -80)),
            new ship_face_normal(31, new(  -7,  -42,    9)),
            new ship_face_normal(31, new(   0,  -30,    6)),
            new ship_face_normal(31, new(   7,  -42,    9)),
        };

        internal static ship_data cobra3a_data = new(
            "Cobra MkIII",
            3,
            0,
            9025,
            21,
            0,
            50,
            150,
            28,
            3,
            9,
            cobra3a_point,
            cobra3a_line,
            cobra3a_face_normal
        );
    }
}