namespace Elite.Ships
{
    using Elite.Structs;

    internal static partial class shipdata
    {
        private static ship_point[] coriolis_point = new ship_point[16]
        {
            new( 160,    0,  160, 31,  1,  0,  6,  2),
            new(   0,  160,  160, 31,  2,  0,  8,  3),
            new(-160,    0,  160, 31,  3,  0,  7,  4),
            new(   0, -160,  160, 31,  1,  0,  5,  4),
            new( 160, -160,    0, 31,  5,  1, 10,  6),
            new( 160,  160,    0, 31,  6,  2, 11,  8),
            new(-160,  160,    0, 31,  7,  3, 12,  8),
            new(-160, -160,    0, 31,  5,  4,  9,  7),
            new( 160,    0, -160, 31, 10,  6, 13, 11),
            new(   0,  160, -160, 31, 11,  8, 13, 12),
            new(-160,    0, -160, 31,  9,  7, 13, 12),
            new(   0, -160, -160, 31,  9,  5, 13, 10),
            new(  10,  -30,  160, 30,  0,  0,  0,  0),
            new(  10,   30,  160, 30,  0,  0,  0,  0),
            new( -10,   30,  160, 30,  0,  0,  0,  0),
            new( -10,  -30,  160, 30,  0,  0,  0,  0),
        };

        private static ship_line[] coriolis_line = new ship_line[28]
        {
            new(31,  1,  0,  0,  3),
            new(31,  2,  0,  0,  1),
            new(31,  3,  0,  1,  2),
            new(31,  4,  0,  2,  3),
            new(31,  5,  1,  3,  4),
            new(31,  6,  1,  0,  4),
            new(31,  6,  2,  0,  5),
            new(31,  8,  2,  5,  1),
            new(31,  8,  3,  1,  6),
            new(31,  7,  3,  2,  6),
            new(31,  7,  4,  2,  7),
            new(31,  5,  4,  3,  7),
            new(31, 13, 10,  8, 11),
            new(31, 13, 11,  8,  9),
            new(31, 13, 12,  9, 10),
            new(31, 13,  9, 10, 11),
            new(31, 10,  5,  4, 11),
            new(31, 10,  6,  4,  8),
            new(31, 11,  6,  5,  8),
            new(31, 11,  8,  5,  9),
            new(31, 12,  8,  6,  9),
            new(31, 12,  7,  6, 10),
            new(31,  9,  7,  7, 10),
            new(31,  9,  5,  7, 11),
            new(30,  0,  0, 12, 13),
            new(30,  0,  0, 13, 14),
            new(30,  0,  0, 14, 15),
            new(30,  0,  0, 15, 12),
        };

        private static ship_face_normal[] coriolis_face_normal = new ship_face_normal[14]
        {
            new ship_face_normal(31,    0,    0,  160),
            new ship_face_normal(31,  107, -107,  107),
            new ship_face_normal(31,  107,  107,  107),
            new ship_face_normal(31, -107,  107,  107),
            new ship_face_normal(31, -107, -107,  107),
            new ship_face_normal(31,    0, -160,    0),
            new ship_face_normal(31,  160,    0,    0),
            new ship_face_normal(31, -160,    0,    0),
            new ship_face_normal(31,    0,  160,    0),
            new ship_face_normal(31, -107, -107, -107),
            new ship_face_normal(31,  107, -107, -107),
            new ship_face_normal(31,  107,  107, -107),
            new ship_face_normal(31, -107,  107, -107),
            new ship_face_normal(31,    0,    0, -160),
        };

        internal static ship_data coriolis_data = new(
            "Coriolis Space Station",
            16, 28, 14,
            0,
            0,
            25600,
            0,
            0,
            120,
            240,
            0,
            6,
            3,
            coriolis_point,
            coriolis_line,
            coriolis_face_normal
        );
    } 
}