// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class NoShip : IShip
    {
        public float Bounty => 0;

        public int EnergyMax => 0;

        public ShipFaceNormal[] FaceNormals => Array.Empty<ShipFaceNormal>();
        public ShipFace[] Faces => Array.Empty<ShipFace>();

        public int LaserFront => 0;

        public int LaserStrength => 0;

        public ShipLine[] Lines => Array.Empty<ShipLine>();
        public int LootMax => 0;

        public int MissilesMax => 0;

        public string Name => "NoShip";

        public ShipPoint[] Points => Array.Empty<ShipPoint>();
        public StockType ScoopedType => StockType.None;
        public float Size => 0;
        public ShipClass Type => ShipClass.None;
        public int VanishPoint => 0;

        public float VelocityMax => 0;
    }
}
