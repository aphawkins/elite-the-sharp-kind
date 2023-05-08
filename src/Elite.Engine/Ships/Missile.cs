// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class Missile : ShipData
    {
        private static readonly ShipFaceNormal[] s_faceNormals =
        {
            new ShipFaceNormal(31, new( -64,    0,   16)),
            new ShipFaceNormal(31, new(   0,  -64,   16)),
            new ShipFaceNormal(31, new(  64,    0,   16)),
            new ShipFaceNormal(31, new(   0,   64,   16)),
            new ShipFaceNormal(31, new(  32,    0,    0)),
            new ShipFaceNormal(31, new(   0,  -32,    0)),
            new ShipFaceNormal(31, new( -32,    0,    0)),
            new ShipFaceNormal(31, new(   0,   32,    0)),
            new ShipFaceNormal(31, new(   0,    0, -176)),
        };

        private static readonly ShipFace[] s_faces =
        {
			//fins
			new(GFX_COL.GFX_COL_RED, new( 0x20, 0x00, 0x00), new[] {  5, 9, 15 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00, 0x20, 0x00), new[] {  15, 9,  5 }),

            new(GFX_COL.GFX_COL_RED, new(-0x20, 0x00, 0x00), new[] {  8, 12, 13 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00, 0x20, 0x00), new[] {  13, 12, 8 }),

            new(GFX_COL.GFX_COL_RED, new(-0x20, 0x00, 0x00), new[] {  7, 11, 14 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00,-0x20, 0x00), new[] { 14, 11, 7 }),

            new(GFX_COL.GFX_COL_RED, new( 0x20, 0x00, 0x00), new[] { 6, 10, 16 }),
            new(GFX_COL.GFX_COL_RED, new( 0x00,-0x20, 0x00), new[] { 16, 10, 6 }),

			//nose cone
			new(GFX_COL.GFX_COL_DARK_RED, new(-0x40, 0x00, 0x10), new[] { 0,  3,  4 }),
            new (GFX_COL.GFX_COL_RED,      new( 0x00,-0x40, 0x10), new[] { 0,  4,  1 }),
            new(GFX_COL.GFX_COL_DARK_RED, new( 0x40, 0x00, 0x10), new[] { 0,  1,  2 }),
            new(GFX_COL.GFX_COL_RED,      new( 0x00, 0x40, 0x10), new[] { 0,  2,  3 }),

			//main body
			new(GFX_COL.GFX_COL_GREY_3, new( 0x20, 0x00, 0x00), new[] { 6,  5,  2, 1 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x00, 0x20, 0x00), new[] { 5,  8,  3, 2 }),
            new(GFX_COL.GFX_COL_GREY_3, new(-0x20, 0x00, 0x00), new[] { 8,  7,  4, 3 }),
            new(GFX_COL.GFX_COL_GREY_1, new( 0x00,-0x20, 0x00), new[] { 7,  6,  1, 4 }),

			//bottom
			new(GFX_COL.GFX_COL_GREY_2, new( 0x00, 0x00,-0xB0), new[] { 5,  6,  7, 8 }),
        };

        private static readonly ShipLine[] s_lines =
        {
            new(31,  2,  1,  0,  1),
            new(31,  3,  2,  0,  2),
            new(31,  3,  0,  0,  3),
            new(31,  1,  0,  0,  4),
            new(31,  2,  4,  1,  2),
            new(31,  5,  1,  1,  4),
            new(31,  6,  0,  3,  4),
            new(31,  7,  3,  2,  3),
            new(31,  7,  4,  2,  5),
            new(31,  5,  4,  1,  6),
            new(31,  6,  5,  4,  7),
            new(31,  7,  6,  3,  8),
            new(31,  8,  6,  7,  8),
            new(31,  8,  7,  5,  8),
            new(31,  8,  4,  5,  6),
            new(31,  8,  5,  6,  7),
            new( 8,  8,  5,  6, 10),
            new( 8,  8,  7,  5,  9),
            new( 8,  8,  7,  8, 12),
            new( 8,  8,  5,  7, 11),
            new( 8,  7,  4,  9, 15),
            new( 8,  5,  4, 10, 16),
            new( 8,  7,  6, 12, 13),
            new( 8,  6,  5, 11, 14),
        };

        private static readonly ShipPoint[] s_points =
                                {
            new(new(   0,    0,   68), 31,  1,  0,  3,  2),
            new(new(   8,   -8,   36), 31,  2,  1,  5,  4),
            new(new(   8,    8,   36), 31,  3,  2,  7,  4),
            new(new(  -8,    8,   36), 31,  3,  0,  7,  6),
            new(new(  -8,   -8,   36), 31,  1,  0,  6,  5),
            new(new(   8,    8,  -44), 31,  7,  4,  8,  8),
            new(new(   8,   -8,  -44), 31,  5,  4,  8,  8),
            new(new(  -8,   -8,  -44), 31,  6,  5,  8,  8),
            new(new(  -8,    8,  -44), 31,  7,  6,  8,  8),
            new(new(  12,   12,  -44),  8,  7,  4,  8,  8),
            new(new(  12,  -12,  -44),  8,  5,  4,  8,  8),
            new(new( -12,  -12,  -44),  8,  6,  5,  8,  8),
            new(new( -12,   12,  -44),  8,  7,  6,  8,  8),
            new(new(  -8,    8,  -12),  8,  7,  6,  7,  7),
            new(new(  -8,   -8,  -12),  8,  6,  5,  6,  6),
            new(new(   8,    8,  -12),  8,  7,  4,  7,  7),
            new(new(   8,   -8,  -12),  8,  5,  4,  5,  5),
        };
        internal Missile() : base(
            "Missile",
            0,
            0,
            1600,
            0,
            0,
            14,
            2,
            44,
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
