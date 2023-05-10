// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal interface IShip
    {
        float Bounty { get; }
        int EnergyMax { get; }
        ShipFaceNormal[] FaceNormals { get; }
        ShipFace[] Faces { get; }
        int LaserFront { get; }
        int LaserStrength { get; }
        ShipLine[] Lines { get; }
        int LootMax { get; }
        int MissilesMax { get; }
        string Name { get; }
        ShipPoint[] Points { get; }
        StockType ScoopedType { get; }
        float Size { get; }
        ShipClass Type { get; }
        int VanishPoint { get; }
        float VelocityMax { get; }
    };
}
