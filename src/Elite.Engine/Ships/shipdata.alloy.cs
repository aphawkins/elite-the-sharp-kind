using Elite.Engine.Enums;
using Elite.Engine.Types;

namespace Elite.Engine.Ships
{
    internal static partial class Ship
    {
        private static readonly ShipPoint[] alloy_point =
        {
            new(new( -15,  -22,   -9), 31, 15, 15, 15, 15),
            new(new( -15,   38,   -9), 31, 15, 15, 15, 15),
            new(new(  19,   32,   11), 20, 15, 15, 15, 15),
            new(new(  10,  -46,    6), 20, 15, 15, 15, 15),
        };

        private static readonly ShipLine[] alloy_line =
        {
            new(31, 15, 15,  0,  1),
            new(16, 15, 15,  1,  2),
            new(20, 15, 15,  2,  3),
            new(16, 15, 15,  3,  0),
        };

        private static readonly ShipFaceNormal[] alloy_face_normal =
        {
            new ShipFaceNormal(0, new(0, 0, 0)),
        };

        private static readonly ShipFace[] alloy_face =
        {
            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00, 0x00), new[] { 0, 1, 2, 3 }),
            new(GFX_COL.GFX_COL_GREY_3, new(0x00, 0x00, 0x00), new[] { 3, 2, 1, 0, 0, 0, 0, 0 }),
        };

        internal static ShipData alloy_data = new(
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
            alloy_point,
            alloy_line,
            alloy_face_normal,
            alloy_face
        );
    }
}