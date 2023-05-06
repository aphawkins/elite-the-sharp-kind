using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal static partial class Ship
    {
        private static readonly ShipPoint[] esccaps_point =
        {
            new(new(  -7,    0,   36), 31,  1,  2,  3,  3),
            new(new(  -7,  -14,  -12), 31,  0,  2,  3,  3),
            new(new(  -7,   14,  -12), 31,  0,  1,  3,  3),
            new(new(  21,    0,    0), 31,  0,  1,  2,  2),
        };

        private static readonly ShipLine[] esccaps_line =
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        private static readonly ShipFaceNormal[] esccaps_face_normal =
        {
            new ShipFaceNormal(31, new(  52,    0, -122)),
            new ShipFaceNormal(31, new(  39,  103,   30)),
            new ShipFaceNormal(31, new(  39, -103,   30)),
            new ShipFaceNormal(31, new(-112,    0,    0)),
        };

        private static readonly ShipFace[] escape_face =
        {
            new ShipFace(GFX_COL.GFX_COL_RED,      new( 0x34, 0x00,-0x7A), new[] { 3, 1, 2 }),
            new ShipFace(GFX_COL.GFX_COL_DARK_RED, new( 0x27, 0x67, 0x1E), new[] { 0, 3, 2 }),
            new ShipFace(GFX_COL.GFX_COL_RED_3,    new( 0x27,-0x67, 0x1E), new[] { 0, 1, 3 }),
            new ShipFace(GFX_COL.GFX_COL_RED_4,    new( 0x70, 0x00, 0x00), new[] { 0, 2, 1 }),
        };

        internal static ShipData esccaps_data = new(
            "Escape Capsule",
            0,
            StockType.Slaves,
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