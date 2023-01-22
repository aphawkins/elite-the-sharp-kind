namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] sidewnd_point = new ship_point[]
        {
            new(new( -32,    0,   36), 31,  1,  0,  5,  4),
            new(new(  32,    0,   36), 31,  2,  0,  6,  5),
            new(new(  64,    0,  -28), 31,  3,  2,  6,  6),
            new(new( -64,    0,  -28), 31,  3,  1,  4,  4),
            new(new(   0,   16,  -28), 31,  1,  0,  3,  2),
            new(new(   0,  -16,  -28), 31,  4,  3,  6,  5),
            new(new( -12,    6,  -28), 15,  3,  3,  3,  3),
            new(new(  12,    6,  -28), 15,  3,  3,  3,  3),
            new(new(  12,   -6,  -28), 12,  3,  3,  3,  3),
            new(new( -12,   -6,  -28), 12,  3,  3,  3,  3),
        };

        internal static ship_line[] sidewnd_line = new ship_line[]
        {
            new(31,  5,  0,  0,  1),
            new(31,  6,  2,  1,  2),
            new(31,  2,  0,  1,  4),
            new(31,  1,  0,  0,  4),
            new(31,  4,  1,  0,  3),
            new(31,  3,  1,  3,  4),
            new(31,  3,  2,  2,  4),
            new(31,  4,  3,  3,  5),
            new(31,  6,  3,  2,  5),
            new(31,  6,  5,  1,  5),
            new(31,  5,  4,  0,  5),
            new(15,  3,  3,  6,  7),
            new(12,  3,  3,  7,  8),
            new(12,  3,  3,  6,  9),
            new(12,  3,  3,  8,  9),
        };

        internal static ship_face_normal[] sidewnd_face_normal = new ship_face_normal[7]
        {
            new(31, new(   0,   32,    8)),
            new(31, new( -12,   47,    6)),
            new(31, new(  12,   47,    6)),
            new(31, new(   0,    0, -112)),
            new(31, new( -12,  -47,    6)),
            new(31, new(   0,  -32,    8)),
            new(31, new(  12,  -47,    6)),
        };

        internal static ship_data sidewnd_data = new(
            "Sidewinder",
            0,
            0,
            4225,
            0,
            5,
            20,
            70,
            37,
            0,
            8,
            sidewnd_point,
            sidewnd_line,
            sidewnd_face_normal);
    }
}