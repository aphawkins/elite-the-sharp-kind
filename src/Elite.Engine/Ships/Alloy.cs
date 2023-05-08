// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class Alloy : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 16;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(0, new(0, 0, 0)),
        };

        public ShipFace[] Faces { get; } =
                                {
            new(GFX_COL.GFX_COL_GREY_1, new(0x00, 0x00, 0x00), new[] { 0, 1, 2, 3 }),
            new(GFX_COL.GFX_COL_GREY_3, new(0x00, 0x00, 0x00), new[] { 3, 2, 1, 0, 0, 0, 0, 0 }),
        };

        public int LaserFront => 0;

        public int LaserStrength => 0;

        public ShipLine[] Lines { get; } =
                        {
            new(31, 15, 15,  0,  1),
            new(16, 15, 15,  1,  2),
            new(20, 15, 15,  2,  3),
            new(16, 15, 15,  3,  0),
        };

        public int LootMax => 0;

        public int MissilesMax => 0;

        public string Name => "Alloy";

        public ShipPoint[] Points { get; } =
                                                        {
            new(new( -15,  -22,   -9), 31, 15, 15, 15, 15),
            new(new( -15,   38,   -9), 31, 15, 15, 15, 15),
            new(new(  19,   32,   11), 20, 15, 15, 15, 15),
            new(new(  10,  -46,    6), 20, 15, 15, 15, 15),
        };
        public StockType ScoopedType => StockType.Alloys;
        public float Size => 100;

        public ShipClass Type => ShipClass.SpaceJunk;

        public int VanishPoint => 5;

        public float VelocityMax => 16;
    }
}
