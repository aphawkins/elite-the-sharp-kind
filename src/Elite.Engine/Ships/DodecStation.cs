// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class DodecStation : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 240;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(   0,    0,  196)),
            new(31, new( 103,  142,   88)),
            new(31, new( 169,  -55,   89)),
            new(31, new(   0, -176,   88)),
            new(31, new(-169,  -55,   89)),
            new(31, new(-103,  142,   88)),
            new(31, new(   0,  176,  -88)),
            new(31, new( 169,   55,  -89)),
            new(31, new( 103, -142,  -88)),
            new(31, new(-103, -142,  -88)),
            new(31, new(-169,   55,  -89)),
            new(31, new(   0,    0, -196)),
        };

        public ShipFace[] Faces { get; } =
        {
            new ShipFace(Colour.Grey4, new(   0x00,  0x00,  0xC4), new[] { 3,  2,  1,  0,  4 }),
            new ShipFace(Colour.Grey1, new(   0x67,  0x8E,  0x58), new[] {  6, 10,  5,  0,  1 }),
            new ShipFace(Colour.Grey2, new(   0xA9, -0x37,  0x59), new[] {  7, 11,  6,  1,  2 }),
            new ShipFace(Colour.Grey3, new(   0x00, -0xB0,  0x58), new[] {   8, 12,  7,  2,  3 }),
            new ShipFace(Colour.Grey1, new(  -0xA9, -0x37,  0x59), new[] { 9, 13,  8,  3,  4 }),
            new ShipFace(Colour.Grey3, new(  -0x67,  0x8E,  0x58), new[] { 5, 14,  9,  4,  0 }),
            new ShipFace(Colour.Grey1, new(   0x00,  0xB0, -0x58), new[] { 15, 19, 14,  5, 10 }),
            new ShipFace(Colour.Grey2, new(   0xA9,  0x37, -0x59), new[] { 16, 15, 10,  6, 11 }),
            new ShipFace(Colour.Grey1, new(   0x67, -0x8E, -0x58), new[] { 17, 16, 11,  7, 12 }),
            new ShipFace(Colour.Grey3, new(  -0x67, -0x8E, -0x58), new[] { 18, 17, 12,  8, 13 }),
            new ShipFace(Colour.Grey2, new(  -0xA9,  0x37, -0x59), new[] { 19, 18, 13,  9, 14 }),
            new ShipFace(Colour.Grey4, new(   0x00,  0x00, -0xC4), new[] { 19, 15, 16, 17, 18 }),
            new ShipFace(Colour.Black, new(    0x00,  0x00,  0xC4), new[] { 22, 20, 21, 23 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 0;

        public ShipLine[] Lines { get; } =
        {
            new(31,  0,  1,  0,  1),
            new(31,  0,  2,  1,  2),
            new(31,  0,  3,  2,  3),
            new(31,  0,  4,  3,  4),
            new(31,  0,  5,  4,  0),
            new(31,  1,  6,  5, 10),
            new(31,  1,  7, 10,  6),
            new(31,  2,  7,  6, 11),
            new(31,  2,  8, 11,  7),
            new(31,  3,  8,  7, 12),
            new(31,  3,  9, 12,  8),
            new(31,  4,  9,  8, 13),
            new(31,  4, 10, 13,  9),
            new(31,  5, 10,  9, 14),
            new(31,  5,  6, 14,  5),
            new(31,  7, 11, 15, 16),
            new(31,  8, 11, 16, 17),
            new(31,  9, 11, 17, 18),
            new(31, 10, 11, 18, 19),
            new(31,  6, 11, 19, 15),
            new(31,  1,  5,  0,  5),
            new(31,  1,  2,  1,  6),
            new(31,  2,  3,  2,  7),
            new(31,  3,  4,  3,  8),
            new(31,  4,  5,  4,  9),
            new(31,  6,  7, 10, 15),
            new(31,  7,  8, 11, 16),
            new(31,  8,  9, 12, 17),
            new(31,  9, 10, 13, 18),
            new(31,  6, 10, 14, 19),
            new(30,  0,  0, 20, 21),
            new(20,  0,  0, 21, 23),
            new(23,  0,  0, 23, 22),
            new(20,  0,  0, 22, 20),
        };

        public int LootMax => 0;

        public int MissilesMax => 0;

        public string Name => "Dodec Space Station";

        public ShipPoint[] Points { get; } =
        {
            new(new(   0,  150,  196), 31,  0,  1,  5,  5),
            new(new( 143,   46,  196), 31,  0,  1,  2,  2),
            new(new(  88, -121,  196), 31,  0,  2,  3,  3),
            new(new( -88, -121,  196), 31,  0,  3,  4,  4),
            new(new(-143,   46,  196), 31,  0,  4,  5,  5),
            new(new(   0,  243,   46), 31,  1,  5,  6,  6),
            new(new( 231,   75,   46), 31,  1,  2,  7,  7),
            new(new( 143, -196,   46), 31,  2,  3,  8,  8),
            new(new(-143, -196,   46), 31,  3,  4,  9,  9),
            new(new(-231,   75,   46), 31,  4,  5, 10, 10),
            new(new( 143,  196,  -46), 31,  1,  6,  7,  7),
            new(new( 231,  -75,  -46), 31,  2,  7,  8,  8),
            new(new(   0, -243,  -46), 31,  3,  8,  9,  9),
            new(new(-231,  -75,  -46), 31,  4,  9, 10, 10),
            new(new(-143,  196,  -46), 31,  5,  6, 10, 10),
            new(new(  88,  121, -196), 31,  6,  7, 11, 11),
            new(new( 143,  -46, -196), 31,  7,  8, 11, 11),
            new(new(   0, -150, -196), 31,  8,  9, 11, 11),
            new(new(-143,  -46, -196), 31,  9, 10, 11, 11),
            new(new( -88,  121, -196), 31,  6, 10, 11, 11),
            new(new( -16,   32,  196), 30,  0,  0,  0,  0),
            new(new( -16,  -32,  196), 30,  0,  0,  0,  0),
            new(new(  16,   32,  196), 23,  0,  0,  0,  0),
            new(new(  16,  -32,  196), 23,  0,  0,  0,  0),
        };

        public StockType ScoopedType => StockType.None;
        public float Size => 32400;
        public ShipClass Type => ShipClass.Station;
        public int VanishPoint => 125;

        public float VelocityMax => 0;
    }
}
