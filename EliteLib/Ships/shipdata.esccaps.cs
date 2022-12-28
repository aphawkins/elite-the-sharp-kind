namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static ship_point[] esccaps_point = new ship_point[4]
        {
            new(  -7,    0,   36, 31,  1,  2,  3,  3),
            new(  -7,  -14,  -12, 31,  0,  2,  3,  3),
            new(  -7,   14,  -12, 31,  0,  1,  3,  3),
            new(  21,    0,    0, 31,  0,  1,  2,  2),
        };

        private static ship_line[] esccaps_line = new ship_line[6]
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        private static ship_face_normal[] esccaps_face_normal = new ship_face_normal[4]
        {
            new ship_face_normal(31,   52,    0, -122),
            new ship_face_normal(31,   39,  103,   30),
            new ship_face_normal(31,   39, -103,   30),
            new ship_face_normal(31, -112,    0,    0),
        };

        internal static ship_data esccaps_data = new(
            "Escape Capsule",
            4, 6, 4,
            0,
            2,
            256,
            0,
            0,
            8,
            17,
            8,
            0,
            0,
            esccaps_point,
            esccaps_line,
            esccaps_face_normal
        );
    }
}