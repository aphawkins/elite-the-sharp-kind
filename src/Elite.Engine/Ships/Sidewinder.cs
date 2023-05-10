// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Sidewinder : IShip
    {
        public float Bounty => 5;

        public int EnergyMax => 70;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(   0,   32,    8)),
            new(31, new( -12,   47,    6)),
            new(31, new(  12,   47,    6)),
            new(31, new(   0,    0, -112)),
            new(31, new( -12,  -47,    6)),
            new(31, new(   0,  -32,    8)),
            new(31, new(  12,  -47,    6)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(Colour.Yellow1, new( 0x00, 0x20, 0x08), new[] { 4, 0, 1 }),
            new(Colour.Gold, new(-0x0C, 0x2F, 0x06), new[] {  4, 3, 0 }),
            new (Colour.Gold, new( 0x0C, 0x2F, 0x06), new[] {  2, 4, 1 }),

            new (Colour.Grey1, new( 0x00, 0x00,-0x70), new[] {  2, 5, 3, 4 }),

            new (Colour.Yellow1, new(-0x0C,-0x2F, 0x06), new[] { 5, 0, 3 }),
            new (Colour.Gold, new( 0x00,-0x20, 0x08), new[] { 1, 0, 5 }),
            new (Colour.Yellow1, new( 0x0C,-0x2F, 0x06), new[] { 2, 1, 5 }),
            new (Colour.Red1, new( 0x00, 0x00,-0x70), new[] { 8, 9, 6, 7 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 8;

        public ShipLine[] Lines { get; } =
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

        public int LootMax => 0;

        public int MissilesMax => 0;

        public string Name => "Sidewinder";

        public ShipPoint[] Points { get; } =
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

        public StockType ScoopedType => StockType.None;
        public float Size => 4225;
        public ShipClass Type => ShipClass.PackHunter;
        public int VanishPoint => 20;

        public float VelocityMax => 37;
    }
}
