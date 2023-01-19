namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] hermit_point = new ship_point[9]
        {
            new(   0,   80,    0, 31, 15, 15, 15, 15),
            new( -80,  -10,    0, 31, 15, 15, 15, 15),
            new(   0,  -80,    0, 31, 15, 15, 15, 15),
            new(  70,  -40,    0, 31, 15, 15, 15, 15),
            new(  60,   50,    0, 31,  6,  5, 13, 12),
            new(  50,    0,   60, 31, 15, 15, 15, 15),
            new( -40,    0,   70, 31,  1,  0,  3,  2),
            new(   0,   30,  -75, 31, 15, 15, 15, 15),
            new(   0,  -50,  -60, 31,  9,  8, 11, 10),
        };

        internal static ship_line[] hermit_line = new ship_line[]
        {
            new(31,  7,  2,  0,  1),
            new(31, 13,  6,  0,  4),
            new(31, 12,  5,  3,  4),
            new(31, 11,  4,  2,  3),
            new(31, 10,  3,  1,  2),
            new(31,  3,  2,  1,  6),
            new(31,  3,  1,  2,  6),
            new(31,  4,  1,  2,  5),
            new(31,  1,  0,  5,  6),
            new(31,  6,  0,  0,  5),
            new(31,  5,  4,  3,  5),
            new(31,  2,  0,  0,  6),
            new(31,  6,  5,  4,  5),
            new(31, 10,  8,  1,  8),
            new(31,  8,  7,  1,  7),
            new(31, 13,  7,  0,  7),
            new(31, 13, 12,  4,  7),
            new(31, 12,  9,  3,  7),
            new(31, 11,  9,  3,  8),
            new(31, 11, 10,  2,  8),
            new(31,  9,  8,  7,  8),
        };

        internal static ship_face_normal[] hermit_face_normal = new ship_face_normal[14]
        {
            new(31,    9,   66,   81),
            new(31,    9,  -66,   81),
            new(31,  -72,   64,   31),
            new(31,  -64,  -73,   47),
            new(31,   45,  -79,   65),
            new(31,  135,   15,   35),
            new(31,   38,   76,   70),
            new(31,  -66,   59,  -39),
            new(31,  -67,  -15,  -80),
            new(31,   66,  -14,  -75),
            new(31,  -70,  -80,  -40),
            new(31,   58, -102,  -51),
            new(31,   81,    9,  -67),
            new(31,   47,   94,  -63),
        };

        internal static ship_data hermit_data = new(
            "Rock Hermit",
            9, 21, 14,
            7,
            0,
            6400,
            0,
            0,
            50,
            180,
            30,
            2,
            1,
            hermit_point,
            hermit_line,
            hermit_face_normal);
    }
}