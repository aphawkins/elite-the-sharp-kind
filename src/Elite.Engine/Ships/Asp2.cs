// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class Asp2 : ShipData
    {
        internal static readonly ShipFaceNormal[] s_faceNormals =
        {
            new(31, new(   0,  -35,    5)),
            new(31, new(   8,  -38,   -7)),
            new(31, new(  -8,  -38,   -7)),
            new(22, new(   0,   24,   -1)),
            new(31, new(   0,   43,   19)),
            new(31, new(  -6,   28,   -2)),
            new(31, new(   6,   28,   -2)),
            new(31, new(  59,  -64,   31)),
            new(31, new( -59,  -64,   31)),
            new(31, new(  80,   46,   50)),
            new(31, new( -80,   46,   50)),
            new(31, new(   0,    0,  -90)),
        };

        internal static readonly ShipLine[] s_lines =
        {
            new(22,  1,  2,  0,  1),
            new(22,  0,  1,  0,  4),
            new(22,  0,  2,  0,  7),
            new(31,  1, 11,  1,  2),
            new(31,  1,  6,  2,  3),
            new(16,  7,  9,  3,  8),
            new(31,  0,  4,  8,  9),
            new(16,  8, 10,  6,  9),
            new(31,  2,  5,  5,  6),
            new(31,  2, 11,  1,  5),
            new(31,  1,  7,  3,  4),
            new(31,  0,  7,  4,  8),
            new(31,  2,  8,  6,  7),
            new(31,  0,  8,  7,  9),
            new(31,  6, 11,  2, 12),
            new(31,  5, 11,  5, 12),
            new(22,  3,  6, 10, 12),
            new(22,  3,  5, 11, 12),
            new(22,  3,  4, 10, 11),
            new(31,  5, 10,  6, 11),
            new(31,  4, 10,  9, 11),
            new(31,  6,  9,  3, 10),
            new(31,  4,  9,  8, 10),
            new(10, 11, 11, 13, 15),
            new( 9, 11, 11, 15, 14),
            new( 8, 11, 11, 14, 16),
            new( 8, 11, 11, 16, 13),
            new(10,  0,  4, 18, 17),
        };

        internal static readonly ShipPoint[] s_points =
                        {
            new(new(   0,  -18,    0), 22,  0,  1,  2,  2),
            new(new(   0,   -9,  -45), 31,  1,  2, 11, 11),
            new(new(  43,    0,  -45), 31,  1,  6, 11, 11),
            new(new(  69,   -3,    0), 31,  1,  6,  7,  9),
            new(new(  43,  -14,   28), 31,  0,  1,  7,  7),
            new(new( -43,    0,  -45), 31,  2,  5, 11, 11),
            new(new( -69,   -3,    0), 31,  2,  5,  8, 10),
            new(new( -43,  -14,   28), 31,  0,  2,  8,  8),
            new(new(  26,   -7,   73), 31,  0,  4,  7,  9),
            new(new( -26,   -7,   73), 31,  0,  4,  8, 10),
            new(new(  43,   14,   28), 31,  3,  4,  6,  9),
            new(new( -43,   14,   28), 31,  3,  4,  5, 10),
            new(new(   0,    9,  -45), 31,  3,  5,  6, 11),
            new(new( -17,    0,  -45), 10, 11, 11, 11, 11),
            new(new(  17,    0,  -45),  9, 11, 11, 11, 11),
            new(new(   0,   -4,  -45), 10, 11, 11, 11, 11),
            new(new(   0,    4,  -45),  8, 11, 11, 11, 11),
            new(new(   0,   -7,   73), 10,  0,  4,  0,  4),
            new(new(   0,   -7,   83), 10,  0,  4,  0,  4),
        };
        private static readonly ShipFace[] s_faces =
        {
            new(GFX_COL.GFX_COL_GREY_4, new( 0x00,-0x23, 0x05), new[] { 8,  9,  7,  0, 4 }),
            new(GFX_COL.GFX_COL_GREY_2, new( 0x08,-0x26,-0x07), new[] { 3,  4,  0,  1, 2 }),
            new (GFX_COL.GFX_COL_GREY_1, new(-0x08,-0x26,-0x07), new[] {  1,  0,  7,  6, 5 }),
            new(GFX_COL.GFX_COL_GREY_3, new( 0x3B,-0x40, 0x1F), new[] {  8,  4, 3 }),
            new(GFX_COL.GFX_COL_GREY_3, new(-0x3B,-0x40, 0x1F), new[] {  6,  7, 9 }),

            new(GFX_COL.GFX_COL_BLUE_2, new( 0x00, 0x18,-0x01), new[] { 11, 10, 12 }),
            new(GFX_COL.GFX_COL_BLUE_1, new( 0x00, 0x2B, 0x13), new[] { 9,  8, 10, 11 }),
            new(GFX_COL.GFX_COL_BLUE_4, new(-0x06, 0x1C,-0x02), new[] { 6, 11, 12, 5 }),
            new(GFX_COL.GFX_COL_BLUE_4, new( 0x06, 0x1C,-0x02), new[] { 2, 12, 10, 3 }),
            new(GFX_COL.GFX_COL_BLUE_3, new( 0x50, 0x2E, 0x32), new[] { 3, 10, 8 }),
            new(GFX_COL.GFX_COL_BLUE_3, new(-0x50, 0x2E, 0x32), new[] { 9, 11, 6 }),

            new(GFX_COL.GFX_COL_DARK_RED, new( 0x00, 0x00,-0x5A), new[] { 2,  1,  5, 12 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00, 0x00,-0x5A), new[] { 14, 15, 13, 16 }),

            new(GFX_COL.GFX_COL_WHITE, new( 0x00, 0x2B, 0x13), new[] { 18, 17 }),
            new(GFX_COL.GFX_COL_WHITE, new( 0x00,-0x23, 0x05), new[] {  17, 18 }),
        };

        internal Asp2() : base(
            "Asp MkII",
            0,
            0,
            3600,
            8,
            20,
            40,
            150,
            40,
            1,
            20,
            s_points,
            s_lines,
            s_faceNormals,
            s_faces
        )
        {
        }
    }
}