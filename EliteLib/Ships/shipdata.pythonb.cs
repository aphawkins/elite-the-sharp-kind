namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        internal static ship_point[] pythonb_point = new ship_point[]
        {
            new(   0,    0,  224, 31,  1,  0,  3,  2),
            new(   0,   48,   48, 31,  1,  0,  5,  4),
            new(  96,    0,  -16, 31, 15, 15, 15, 15),
            new( -96,    0,  -16, 31, 15, 15, 15, 15),
            new(   0,   48,  -32, 31,  5,  4,  9,  8),
            new(   0,   24, -112, 31,  8,  9, 12, 12),
            new( -48,    0, -112, 31, 11,  8, 12, 12),
            new(  48,    0, -112, 31, 10,  9, 12, 12),
            new(   0,  -48,   48, 31,  3,  2,  7,  6),
            new(   0,  -48,  -32, 31,  7,  6, 11, 10),
            new(   0,  -24, -112, 31, 11, 10, 12, 12),
        };

        internal static ship_line[] pythonb_line = new ship_line[26]
        {
            new(31,  3,  2,  0,  8),
            new(31,  2,  0,  0,  3),
            new(31,  3,  1,  0,  2),
            new(31,  1,  0,  0,  1),
            new(31,  5,  9,  2,  4),
            new(31,  5,  1,  1,  2),
            new(31,  3,  7,  2,  8),
            new(31,  4,  0,  1,  3),
            new(31,  6,  2,  3,  8),
            new(31, 10,  7,  2,  9),
            new(31,  8,  4,  3,  4),
            new(31, 11,  6,  3,  9),
            new( 7,  8,  8,  3,  5),
            new( 7, 11, 11,  3, 10),
            new( 7,  9,  9,  2,  5),
            new( 7, 10, 10,  2, 10),
            new(31, 10,  9,  2,  7),
            new(31, 11,  8,  3,  6),
            new(31, 12,  8,  5,  6),
            new(31, 12,  9,  5,  7),
            new(31, 10, 12,  7, 10),
            new(31, 12, 11,  6, 10),
            new(31,  9,  8,  4,  5),
            new(31, 11, 10,  9, 10),
            new(31,  5,  4,  1,  4),
            new(31,  7,  6,  8,  9),
        };

        internal static ship_face_normal[] pythonb_face_normal = new ship_face_normal[13]
        {
            new(31,  -27,   40,   11),
            new(31,   27,   40,   11),
            new(31,  -27,  -40,   11),
            new(31,   27,  -40,   11),
            new(31,  -19,   38,    0),
            new(31,   19,   38,    0),
            new(31,  -19,  -38,    0),
            new(31,   19,  -38,    0),
            new(31,  -25,   37,  -11),
            new(31,   25,   37,  -11),
            new(31,   25,  -37,  -11),
            new(31,  -25,  -37,  -11),
            new(31,    0,    0, -112),
        };

        internal static ship_data pythonb_data = new(
            "Python",
            11, 26, 13,
            2,
            0,
            6400,
            0,
            200,
            40,
            250,
            20,
            3,
            13,
            pythonb_point,
            pythonb_line,
            pythonb_face_normal);
    }
}