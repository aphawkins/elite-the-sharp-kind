namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] thargon_point = new ship_point[]
        {
            new(  -9,    0,   40, 31,  0,  1,  5,  5),
            new(  -9,  -38,   12, 31,  0,  1,  2,  2),
            new(  -9,  -24,  -32, 31,  0,  2,  3,  3),
            new(  -9,   24,  -32, 31,  0,  3,  4,  4),
            new(  -9,   38,   12, 31,  0,  4,  5,  5),
            new(   9,    0,   -8, 31,  1,  5,  6,  6),
            new(   9,  -10,  -15, 31,  1,  2,  6,  6),
            new(   9,   -6,  -26, 31,  2,  3,  6,  6),
            new(   9,    6,  -26, 31,  3,  4,  6,  6),
            new(   9,   10,  -15, 31,  4,  5,  6,  6),
        };

        internal static ship_line[] thargon_line = new ship_line[]
        {
            new(31,  1,  0,  0,  1),
            new(31,  2,  0,  1,  2),
            new(31,  3,  0,  2,  3),
            new(31,  4,  0,  3,  4),
            new(31,  5,  0,  0,  4),
            new(31,  5,  1,  0,  5),
            new(31,  2,  1,  1,  6),
            new(31,  3,  2,  2,  7),
            new(31,  4,  3,  3,  8),
            new(31,  5,  4,  4,  9),
            new(31,  6,  1,  5,  6),
            new(31,  6,  2,  6,  7),
            new(31,  6,  3,  7,  8),
            new(31,  6,  4,  8,  9),
            new(31,  6,  5,  9,  5),
        };

        internal static ship_face_normal[] thargon_face_normal = new ship_face_normal[7]
        {
            new(31,  -36,    0,    0),
            new(31,   20,   -5,    7),
            new(31,   46,  -42,  -14),
            new(31,   36,    0, -104),
            new(31,   46,   42,  -14),
            new(31,   20,    5,    7),
            new(31,   36,    0,    0),
        };

        internal static ship_data thargon_data = new(
            "Thargon",
            0,
            15,
            1600,
            0,
            5,
            20,
            20,
            30,
            0,
            8,
            thargon_point,
            thargon_line,
            thargon_face_normal);
    }
}