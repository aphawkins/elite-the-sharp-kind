// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal interface IShip
    {
        string Name { get; }
        ShipClass Type { get; }
        float Size { get; }
        int EnergyMax { get; }
        int MissilesMax { get; }
        int LootMax { get; }
        float Bounty { get; }
        StockType ScoopedType { get; }
        int LaserFront { get; }
        int VanishPoint { get; }
        float VelocityMax { get; }
        int LaserStrength { get; }
        ShipPoint[] Points { get; }
        ShipLine[] Lines { get; }
        ShipFaceNormal[] FaceNormals { get; }
        ShipFace[] Faces { get; }
    };
}
