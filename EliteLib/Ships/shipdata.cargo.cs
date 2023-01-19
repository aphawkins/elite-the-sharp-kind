namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static ship_point[] cargo_point = new ship_point[]
        {
            new(  24,   16,    0, 31,  1,  0,  5,  5),
            new(  24,    5,   15, 31,  1,  0,  2,  2),
            new(  24,  -13,    9, 31,  2,  0,  3,  3),
            new(  24,  -13,   -9, 31,  3,  0,  4,  4),
            new(  24,    5,  -15, 31,  4,  0,  5,  5),
            new( -24,   16,    0, 31,  5,  1,  6,  6),
            new( -24,    5,   15, 31,  2,  1,  6,  6),
            new( -24,  -13,    9, 31,  3,  2,  6,  6),
            new( -24,  -13,   -9, 31,  4,  3,  6,  6),
            new( -24,    5,  -15, 31,  5,  4,  6,  6),
        };

        private static ship_line[] cargo_line = new ship_line[]
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

        private static ship_face_normal[] cargo_face_normal = new ship_face_normal[7]
        {
            new ship_face_normal(31,   96,    0,    0),
            new ship_face_normal(31,    0,   41,   30),
            new ship_face_normal(31,    0,  -18,   48),
            new ship_face_normal(31,    0,  -51,    0),
            new ship_face_normal(31,    0,  -18,  -48),
            new ship_face_normal(31,    0,   41,  -30),
            new ship_face_normal(31,  -96,    0,    0),
        };

        internal static ship_data cargo_data = new(
            "Cargo Canister",
            10, 15, 7,
            0,
            0,
            400,
            0,
            0,
            12,
            17,
            15,
            0,
            0,
            cargo_point,
            cargo_line,
            cargo_face_normal
        );
    }
}