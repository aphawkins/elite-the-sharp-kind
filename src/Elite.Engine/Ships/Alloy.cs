// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class Alloy : ShipData
    {
        private static readonly ShipFace[] s_faces =
        {
            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00, 0x00), new[] { 0, 1, 2, 3 }),
            new(GFX_COL.GFX_COL_GREY_3, new(0x00, 0x00, 0x00), new[] { 3, 2, 1, 0, 0, 0, 0, 0 }),
        };

        private static readonly ShipFaceNormal[] s_faceNormals =
        {
            new ShipFaceNormal(0, new(0, 0, 0)),
        };

        private static readonly ShipLine[] s_lines =
        {
            new(31, 15, 15,  0,  1),
            new(16, 15, 15,  1,  2),
            new(20, 15, 15,  2,  3),
            new(16, 15, 15,  3,  0),
        };

        private static readonly ShipPoint[] s_points =
                                {
            new(new( -15,  -22,   -9), 31, 15, 15, 15, 15),
            new(new( -15,   38,   -9), 31, 15, 15, 15, 15),
            new(new(  19,   32,   11), 20, 15, 15, 15, 15),
            new(new(  10,  -46,    6), 20, 15, 15, 15, 15),
        };
        internal Alloy() : base(
            "Alloy",
            0,
            StockType.Alloys,
            100,
            0,
            0,
            5,
            16,
            16,
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
