namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static ship_point[] missile_point = new ship_point[]
        {
            new(new(   0,    0,   68), 31,  1,  0,  3,  2),
            new(new(   8,   -8,   36), 31,  2,  1,  5,  4),
            new(new(   8,    8,   36), 31,  3,  2,  7,  4),
            new(new(  -8,    8,   36), 31,  3,  0,  7,  6),
            new(new(  -8,   -8,   36), 31,  1,  0,  6,  5),
            new(new(   8,    8,  -44), 31,  7,  4,  8,  8),
            new(new(   8,   -8,  -44), 31,  5,  4,  8,  8),
            new(new(  -8,   -8,  -44), 31,  6,  5,  8,  8),
            new(new(  -8,    8,  -44), 31,  7,  6,  8,  8),
            new(new(  12,   12,  -44),  8,  7,  4,  8,  8),
            new(new(  12,  -12,  -44),  8,  5,  4,  8,  8),
            new(new( -12,  -12,  -44),  8,  6,  5,  8,  8),
            new(new( -12,   12,  -44),  8,  7,  6,  8,  8),
            new(new(  -8,    8,  -12),  8,  7,  6,  7,  7),
            new(new(  -8,   -8,  -12),  8,  6,  5,  6,  6),
            new(new(   8,    8,  -12),  8,  7,  4,  7,  7),
            new(new(   8,   -8,  -12),  8,  5,  4,  5,  5),
        };

        private static ship_line[] missile_line = new ship_line[]
        {
            new(31,  2,  1,  0,  1),
            new(31,  3,  2,  0,  2),
            new(31,  3,  0,  0,  3),
            new(31,  1,  0,  0,  4),
            new(31,  2,  4,  1,  2),
            new(31,  5,  1,  1,  4),
            new(31,  6,  0,  3,  4),
            new(31,  7,  3,  2,  3),
            new(31,  7,  4,  2,  5),
            new(31,  5,  4,  1,  6),
            new(31,  6,  5,  4,  7),
            new(31,  7,  6,  3,  8),
            new(31,  8,  6,  7,  8),
            new(31,  8,  7,  5,  8),
            new(31,  8,  4,  5,  6),
            new(31,  8,  5,  6,  7),
            new( 8,  8,  5,  6, 10),
            new( 8,  8,  7,  5,  9),
            new( 8,  8,  7,  8, 12),
            new( 8,  8,  5,  7, 11),
            new( 8,  7,  4,  9, 15),
            new( 8,  5,  4, 10, 16),
            new( 8,  7,  6, 12, 13),
            new( 8,  6,  5, 11, 14),
        };

        private static ship_face_normal[] missile_face_normal = new ship_face_normal[9]
        {
            new ship_face_normal(31, new( -64,    0,   16)),
            new ship_face_normal(31, new(   0,  -64,   16)),
            new ship_face_normal(31, new(  64,    0,   16)),
            new ship_face_normal(31, new(   0,   64,   16)),
            new ship_face_normal(31, new(  32,    0,    0)),
            new ship_face_normal(31, new(   0,  -32,    0)),
            new ship_face_normal(31, new( -32,    0,    0)),
            new ship_face_normal(31, new(   0,   32,    0)),
            new ship_face_normal(31, new(   0,    0, -176)),
        };

        internal static ship_data missile_data = new(
            "Missile",
            0,
            0,
            1600,
            0,
            0,
            14,
            2,
            44,
            0,
            0,
            missile_point,
            missile_line,
            missile_face_normal
        );
    }
}