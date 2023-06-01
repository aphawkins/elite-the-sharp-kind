// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Trader;

namespace EliteSharp.Ships
{
    internal interface IObject
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

        ShipClass Class { get; }

        int VanishPoint { get; }

        float VelocityMax { get; }

        int Acceleration { get; set; }

        int Bravery { get; set; }

        int Energy { get; set; }

        int ExpDelta { get; set; }

        ShipFlags Flags { get; set; }

        Vector3 Location { get; set; }

        int Missiles { get; set; }

        Vector3[] Rotmat { get; set; }

        float RotX { get; set; }

        float RotZ { get; set; }

        IObject? Target { get; set; }

        ShipType Type { get; }

        float Velocity { get; set; }
    }
}
