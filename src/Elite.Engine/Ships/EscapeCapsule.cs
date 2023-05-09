// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal sealed class EscapeCapsule : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 17;

        public ShipFaceNormal[] FaceNormals { get; } =
        {
            new(31, new(  52,    0, -122)),
            new(31, new(  39,  103,   30)),
            new(31, new(  39, -103,   30)),
            new(31, new(-112,    0,    0)),
        };

        public ShipFace[] Faces { get; } =
        {
            new ShipFace(GFX_COL.GFX_COL_RED,      new( 0x34, 0x00,-0x7A), new[] { 3, 1, 2 }),
            new ShipFace(GFX_COL.GFX_COL_DARK_RED, new( 0x27, 0x67, 0x1E), new[] { 0, 3, 2 }),
            new ShipFace(GFX_COL.GFX_COL_RED_3,    new( 0x27,-0x67, 0x1E), new[] { 0, 1, 3 }),
            new ShipFace(GFX_COL.GFX_COL_RED_4,    new( 0x70, 0x00, 0x00), new[] { 0, 2, 1 }),
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

        public string Name => "Escape Capsule";

        public ShipPoint[] Points { get; } =
                                                                                        {
            new(new(  -7,    0,   36), 31,  1,  2,  3,  3),
            new(new(  -7,  -14,  -12), 31,  0,  2,  3,  3),
            new(new(  -7,   14,  -12), 31,  0,  1,  3,  3),
            new(new(  21,    0,    0), 31,  0,  1,  2,  2),
        };
        public StockType ScoopedType => StockType.Slaves;
        public float Size => 256;
        public ShipClass Type => ShipClass.SpaceJunk;
        public int VanishPoint => 8;

        public float VelocityMax => 8;
    }
}
