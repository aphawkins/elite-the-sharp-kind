// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Trader;

namespace EliteSharp.Ships
{
    internal interface IShip
    {
        int Acceleration { get; set; }

        float Bounty { get; }

        int Bravery { get; set; }

        ShipClass Class { get; }

        int Energy { get; set; }

        int EnergyMax { get; }

        int ExpDelta { get; set; }

        ShipFaceNormal[] FaceNormals { get; }

        ShipFace[] Faces { get; }

        ShipFlags Flags { get; set; }

        int LaserFront { get; }

        int LaserStrength { get; }

        ShipLine[] Lines { get; }

        Vector3 Location { get; set; }

        int LootMax { get; }

        float MinDistance { get; }

        int Missiles { get; set; }

        int MissilesMax { get; }

        string Name { get; }

        ShipPoint[] Points { get; }

        Vector3[] Rotmat { get; set; }

        float RotX { get; set; }

        float RotZ { get; set; }

        StockType ScoopedType { get; }

        float Size { get; }

        IShip? Target { get; set; }

        ShipType Type { get; }

        int VanishPoint { get; }

        float Velocity { get; set; }

        float VelocityMax { get; }

        IShip Clone();

        void Draw();
    }
}
