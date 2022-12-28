namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] viper_point = new ship_point[15]
        {
            new(   0,    0,   72, 31,  2,  1,  4,  3),
            new(   0,   16,   24, 30,  1,  0,  2,  2),
            new(   0,  -16,   24, 30,  4,  3,  5,  5),
            new(  48,    0,  -24, 31,  4,  2,  6,  6),
            new( -48,    0,  -24, 31,  3,  1,  6,  6),
            new(  24,  -16,  -24, 30,  5,  4,  6,  6),
            new( -24,  -16,  -24, 30,  3,  5,  6,  6),
            new(  24,   16,  -24, 31,  2,  0,  6,  6),
            new( -24,   16,  -24, 31,  1,  0,  6,  6),
            new( -32,    0,  -24, 19,  6,  6,  6,  6),
            new(  32,    0,  -24, 19,  6,  6,  6,  6),
            new(   8,    8,  -24, 19,  6,  6,  6,  6),
            new(  -8,    8,  -24, 19,  6,  6,  6,  6),
            new(  -8,   -8,  -24, 18,  6,  6,  6,  6),
            new(   8,   -8,  -24, 18,  6,  6,  6,  6),
        };

        internal static ship_line[] viper_line = new ship_line[20]
        {
            new(31,  4,  2,  0,  3),
            new(30,  2,  1,  0,  1),
            new(30,  4,  3,  0,  2),
            new(31,  3,  1,  0,  4),
            new(30,  2,  0,  1,  7),
            new(30,  1,  0,  1,  8),
            new(30,  5,  4,  2,  5),
            new(30,  5,  3,  2,  6),
            new(31,  6,  0,  7,  8),
            new(30,  6,  5,  5,  6),
            new(31,  6,  1,  4,  8),
            new(30,  6,  3,  4,  6),
            new(31,  6,  2,  3,  7),
            new(30,  4,  6,  3,  5),
            new(19,  6,  6,  9, 12),
            new(18,  6,  6,  9, 13),
            new(19,  6,  6, 10, 11),
            new(18,  6,  6, 10, 14),
            new(16,  6,  6, 11, 14),
            new(16,  6,  6, 12, 13),
        };

        internal static ship_face_normal[] viper_face_normal = new ship_face_normal[7]
        {
            new(31,    0,   32,    0),
            new(31,  -22,   33,   11),
            new(31,   22,   33,   11),
            new(31,  -22,  -33,   11),
            new(31,   22,  -33,   11),
            new(31,    0,  -32,    0),
            new(31,    0,    0,  -48),
        };

        internal static ship_data viper_data = new(
            "Viper",
            15, 20, 7,
            0,
            0,
            5625,
            0,
            0,
            23,
            140,
            32,
            1,
            8,
            viper_point,
            viper_line,
            viper_face_normal);
    }
}