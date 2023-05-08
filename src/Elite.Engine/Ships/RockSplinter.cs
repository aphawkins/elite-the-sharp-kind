// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class RockSplinter : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 20;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(18, new(  30,    0,    0)),
            new(20, new(  22,   32,   -8)),
            new( 0, new(   0,    2,    0)),
            new( 0, new(  17,   23,   95)),
        };

        public ShipFace[] Faces { get; } =
        {
            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00, 0x00 ), new[] { 3, 2, 1 }),
            new(GFX_COL.GFX_COL_GREY_2, new(0x00, 0x00, 0x00 ), new[] { 0, 2, 3 }),
            new(GFX_COL.GFX_COL_GREY_3, new(0x00, 0x00, 0x00 ), new[] { 3, 1 , 0 }),
            new(GFX_COL.GFX_COL_GREY_4, new(0x00, 0x00, 0x00 ), new[] { 0, 1, 2 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 0;

        public ShipLine[] Lines { get; } =
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        public int LootMax => 0;

        public int MissilesMax => 0;

        public string Name => "Rock Splinter";

        public ShipPoint[] Points { get; } =
                                                                                        {
            new(new( -24,  -25,   16), 31,  1,  2,  3,  3),
            new(new(   0,   12,  -10), 31,  0,  2,  3,  3),
            new(new(  11,   -6,    2), 31,  0,  1,  3,  3),
            new(new(  12,   42,    7), 31,  0,  1,  2,  2),
        };
        public StockType ScoopedType => StockType.Minerals;
        public float Size => 256;
        public ShipClass Type => throw new NotImplementedException();
        public int VanishPoint => 8;

        public float VelocityMax => 10;
    }
}
