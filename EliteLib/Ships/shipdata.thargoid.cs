namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] thargoid_point = new ship_point[]
        {
            new(  32,  -48,   48, 31,  4,  0,  8,  8),
            new(  32,  -68,    0, 31,  1,  0,  4,  4),
            new(  32,  -48,  -48, 31,  2,  1,  4,  4),
            new(  32,    0,  -68, 31,  3,  2,  4,  4),
            new(  32,   48,  -48, 31,  4,  3,  5,  5),
            new(  32,   68,    0, 31,  5,  4,  6,  6),
            new(  32,   48,   48, 31,  6,  4,  7,  7),
            new(  32,    0,   68, 31,  7,  4,  8,  8),
            new( -24, -116,  116, 31,  8,  0,  9,  9),
            new( -24, -164,    0, 31,  1,  0,  9,  9),
            new( -24, -116, -116, 31,  2,  1,  9,  9),
            new( -24,    0, -164, 31,  3,  2,  9,  9),
            new( -24,  116, -116, 31,  5,  3,  9,  9),
            new( -24,  164,    0, 31,  6,  5,  9,  9),
            new( -24,  116,  116, 31,  7,  6,  9,  9),
            new( -24,    0,  164, 31,  8,  7,  9,  9),
            new( -24,   64,   80, 30,  9,  9,  9,  9),
            new( -24,   64,  -80, 30,  9,  9,  9,  9),
            new( -24,  -64,  -80, 30,  9,  9,  9,  9),
            new( -24,  -64,   80, 30,  9,  9,  9,  9),
        };

        internal static ship_line[] thargoid_line = new ship_line[]
        {
            new(31,  8,  4,  0,  7),
            new(31,  4,  0,  0,  1),
            new(31,  4,  1,  1,  2),
            new(31,  4,  2,  2,  3),
            new(31,  4,  3,  3,  4),
            new(31,  5,  4,  4,  5),
            new(31,  6,  4,  5,  6),
            new(31,  7,  4,  6,  7),
            new(31,  8,  0,  0,  8),
            new(31,  1,  0,  1,  9),
            new(31,  2,  1,  2, 10),
            new(31,  3,  2,  3, 11),
            new(31,  5,  3,  4, 12),
            new(31,  6,  5,  5, 13),
            new(31,  7,  6,  6, 14),
            new(31,  8,  7,  7, 15),
            new(31,  9,  8,  8, 15),
            new(31,  9,  0,  8,  9),
            new(31,  9,  1,  9, 10),
            new(31,  9,  2, 10, 11),
            new(31,  9,  3, 11, 12),
            new(31,  9,  5, 12, 13),
            new(31,  9,  6, 13, 14),
            new(31,  9,  7, 14, 15),
            new(30,  9,  9, 16, 17),
            new(30,  9,  9, 18, 19),
        };

        internal static ship_face_normal[] thargoid_face_normal = new ship_face_normal[10]
        {
            new(31,  103,  -60,   25),
            new(31,  103,  -60,  -25),
            new(31,  103,  -25,  -60),
            new(31,  103,   25,  -60),
            new(31,   64,    0,    0),
            new(31,  103,   60,  -25),
            new(31,  103,   60,   25),
            new(31,  103,   25,   60),
            new(31,  103,  -25,   60),
            new(31,  -48,    0,    0),
        };

        internal static ship_data thargoid_data = new(
            "Thargoid",
            0,
            0,
            9801,
            15,
            50,
            55,
            240,
            39,
            6,
            11,
            thargoid_point,
            thargoid_line,
            thargoid_face_normal);
    }
}