// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Trader;

namespace EliteSharp.Ships
{
    internal interface IShip : IObject
    {
        int Acceleration { get; set; }

        float Bounty { get; set; }

        int Bravery { get; set; }

        int Energy { get; set; }

        int EnergyMax { get; set; }

        int ExpDelta { get; set; }

        ShipFaceNormal[] FaceNormals { get; set; }

        ShipFace[] Faces { get; set; }

        int LaserFront { get; set; }

        int LaserStrength { get; set; }

        ShipLine[] Lines { get; set; }

        int LootMax { get; set; }

        float MinDistance { get; set; }

        int Missiles { get; set; }

        int MissilesMax { get; set; }

        string Name { get; set; }

        ShipPoint[] Points { get; set; }

        StockType ScoopedType { get; set; }

        float Size { get; set; }

        IObject? Target { get; set; }

        int VanishPoint { get; set; }

        float Velocity { get; set; }

        float VelocityMax { get; set; }
    }
}
