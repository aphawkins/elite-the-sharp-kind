namespace Elite.Engine.Ships
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static partial class shipdata
    {
        private static readonly ship_point[] esccaps_point =
        {
            new(new(  -7,    0,   36), 31,  1,  2,  3,  3),
            new(new(  -7,  -14,  -12), 31,  0,  2,  3,  3),
            new(new(  -7,   14,  -12), 31,  0,  1,  3,  3),
            new(new(  21,    0,    0), 31,  0,  1,  2,  2),
        };

        private static readonly ship_line[] esccaps_line =
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        private static readonly ship_face_normal[] esccaps_face_normal =
        {
            new ship_face_normal(31, new(  52,    0, -122)),
            new ship_face_normal(31, new(  39,  103,   30)),
            new ship_face_normal(31, new(  39, -103,   30)),
            new ship_face_normal(31, new(-112,    0,    0)),
        };

        private static readonly ship_face[] escape_face =
        {
            new ship_face(GFX_COL.GFX_COL_RED,      new( 0x34, 0x00,-0x7A), new[] { 3, 1, 2 }),
            new ship_face(GFX_COL.GFX_COL_DARK_RED, new( 0x27, 0x67, 0x1E), new[] { 0, 3, 2 }),
            new ship_face(GFX_COL.GFX_COL_RED_3,    new( 0x27,-0x67, 0x1E), new[] { 0, 1, 3 }),
            new ship_face(GFX_COL.GFX_COL_RED_4,    new( 0x70, 0x00, 0x00), new[] { 0, 2, 1 }),
        };

        internal static ship_data esccaps_data = new(
            "Escape Capsule",
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
            esccaps_face_normal,
            escape_face
        );
    }
}