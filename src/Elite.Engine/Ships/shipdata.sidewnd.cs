using Elite.Engine.Enums;
using Elite.Engine.Types;

namespace Elite.Engine.Ships
{
    internal static partial class Ship
    {
        internal static readonly ShipPoint[] sidewnd_point =
        {
            new(new( -32,    0,   36), 31,  1,  0,  5,  4),
            new(new(  32,    0,   36), 31,  2,  0,  6,  5),
            new(new(  64,    0,  -28), 31,  3,  2,  6,  6),
            new(new( -64,    0,  -28), 31,  3,  1,  4,  4),
            new(new(   0,   16,  -28), 31,  1,  0,  3,  2),
            new(new(   0,  -16,  -28), 31,  4,  3,  6,  5),
            new(new( -12,    6,  -28), 15,  3,  3,  3,  3),
            new(new(  12,    6,  -28), 15,  3,  3,  3,  3),
            new(new(  12,   -6,  -28), 12,  3,  3,  3,  3),
            new(new( -12,   -6,  -28), 12,  3,  3,  3,  3),
        };

        internal static readonly ShipLine[] sidewnd_line =
        {
            new(31,  5,  0,  0,  1),
            new(31,  6,  2,  1,  2),
            new(31,  2,  0,  1,  4),
            new(31,  1,  0,  0,  4),
            new(31,  4,  1,  0,  3),
            new(31,  3,  1,  3,  4),
            new(31,  3,  2,  2,  4),
            new(31,  4,  3,  3,  5),
            new(31,  6,  3,  2,  5),
            new(31,  6,  5,  1,  5),
            new(31,  5,  4,  0,  5),
            new(15,  3,  3,  6,  7),
            new(12,  3,  3,  7,  8),
            new(12,  3,  3,  6,  9),
            new(12,  3,  3,  8,  9),
        };

        internal static readonly ShipFaceNormal[] sidewnd_face_normal =
        {
            new(31, new(   0,   32,    8)),
            new(31, new( -12,   47,    6)),
            new(31, new(  12,   47,    6)),
            new(31, new(   0,    0, -112)),
            new(31, new( -12,  -47,    6)),
            new(31, new(   0,  -32,    8)),
            new(31, new(  12,  -47,    6)),
        };

        private static readonly ShipFace[] sidewinder_face =
        {
            new(GFX_COL.GFX_COL_YELLOW_1, new( 0x00, 0x20, 0x08), new[] { 4, 0, 1 }),
            new(GFX_COL.GFX_COL_GOLD, new(-0x0C, 0x2F, 0x06), new[] {  4, 3, 0 }),
            new (GFX_COL.GFX_COL_GOLD, new( 0x0C, 0x2F, 0x06), new[] {  2, 4, 1 }),

            new (GFX_COL.GFX_COL_GREY_1, new( 0x00, 0x00,-0x70), new[] {  2, 5, 3, 4 }),

            new (GFX_COL.GFX_COL_YELLOW_1, new(-0x0C,-0x2F, 0x06), new[] { 5, 0, 3 }),
            new (GFX_COL.GFX_COL_GOLD, new( 0x00,-0x20, 0x08), new[] { 1, 0, 5 }),
            new (GFX_COL.GFX_COL_YELLOW_1, new( 0x0C,-0x2F, 0x06), new[] { 2, 1, 5 }),
            new (GFX_COL.GFX_COL_RED, new( 0x00, 0x00,-0x70), new[] { 8, 9, 6, 7 }),
        };

        internal static ShipData sidewnd_data = new(
            "Sidewinder",
            0,
            0,
            4225,
            0,
            5,
            20,
            70,
            37,
            0,
            8,
            sidewnd_point,
            sidewnd_line,
            sidewnd_face_normal,
            sidewinder_face
        );
    }
}