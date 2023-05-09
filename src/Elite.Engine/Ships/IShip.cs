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
