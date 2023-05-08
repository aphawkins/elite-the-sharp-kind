using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class EscapeCapsule : ShipData
    {
        private static readonly ShipPoint[] s_points =
        {
            new(new(  -7,    0,   36), 31,  1,  2,  3,  3),
            new(new(  -7,  -14,  -12), 31,  0,  2,  3,  3),
            new(new(  -7,   14,  -12), 31,  0,  1,  3,  3),
            new(new(  21,    0,    0), 31,  0,  1,  2,  2),
        };

        private static readonly ShipLine[] s_lines =
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        private static readonly ShipFaceNormal[] s_faceNormals =
        {
            new ShipFaceNormal(31, new(  52,    0, -122)),
            new ShipFaceNormal(31, new(  39,  103,   30)),
            new ShipFaceNormal(31, new(  39, -103,   30)),
            new ShipFaceNormal(31, new(-112,    0,    0)),
        };

        private static readonly ShipFace[] s_faces =
        {
            new ShipFace(GFX_COL.GFX_COL_RED,      new( 0x34, 0x00,-0x7A), new[] { 3, 1, 2 }),
            new ShipFace(GFX_COL.GFX_COL_DARK_RED, new( 0x27, 0x67, 0x1E), new[] { 0, 3, 2 }),
            new ShipFace(GFX_COL.GFX_COL_RED_3,    new( 0x27,-0x67, 0x1E), new[] { 0, 1, 3 }),
            new ShipFace(GFX_COL.GFX_COL_RED_4,    new( 0x70, 0x00, 0x00), new[] { 0, 2, 1 }),
        };

        internal EscapeCapsule() : base(
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
            s_points,
            s_lines,
            s_faceNormals,
            s_faces
        )
        {
        }
    }
}
