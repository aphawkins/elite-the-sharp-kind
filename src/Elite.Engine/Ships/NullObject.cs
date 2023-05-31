// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal class NullObject : IObject
    {
        internal NullObject()
        {
        }

        internal NullObject(IObject other)
        {
            Bounty = other.Bounty;
            EnergyMax = other.EnergyMax;
            FaceNormals = other.FaceNormals;
            Faces = other.Faces;
            LaserFront = other.LaserFront;
            LaserStrength = other.LaserStrength;
            Lines = other.Lines;
            LootMax = other.LootMax;
            MissilesMax = other.MissilesMax;
            Name = other.Name;
            Points = other.Points;
            ScoopedType = other.ScoopedType;
            Size = other.Size;
            Class = other.Class;
            VanishPoint = other.VanishPoint;
            VelocityMax = other.VelocityMax;
            ExpDelta = other.ExpDelta;
            Flags = other.Flags;
            Type = other.Type;
            Location = other.Location.Cloner();
            Rotmat = other.Rotmat.Cloner();
            RotX = other.RotX;
            RotZ = other.RotZ;
            Flags = other.Flags;
            Energy = other.Energy;
            Velocity = other.Velocity;
            Acceleration = other.Acceleration;
            Missiles = other.Missiles;
            Target = other.Target;
            Bravery = other.Bravery;
            ExpDelta = other.ExpDelta;
        }

        public int Acceleration { get; set; }

        public float Bounty { get; protected set; }

        public int Bravery { get; set; }

        public ShipClass Class { get; protected set; }

        public int Energy { get; set; }

        public int EnergyMax { get; protected set; }

        public int ExpDelta { get; set; }

        public ShipFaceNormal[] FaceNormals { get; protected set; } = Array.Empty<ShipFaceNormal>();

        public ShipFace[] Faces { get; protected set; } = Array.Empty<ShipFace>();

        public ShipFlags Flags { get; set; } = ShipFlags.None;

        public int LaserFront { get; protected set; }

        public int LaserStrength { get; protected set; }

        public ShipLine[] Lines { get; protected set; } = Array.Empty<ShipLine>();

        public Vector3 Location { get; set; }

        public int LootMax { get; protected set; }

        public int Missiles { get; set; }

        public int MissilesMax { get; protected set; }

        public string Name { get; protected set; } = string.Empty;

        public ShipPoint[] Points { get; protected set; } = Array.Empty<ShipPoint>();

        public Vector3[] Rotmat { get; set; } = new Vector3[3];

        public float RotX { get; set; }

        public float RotZ { get; set; }

        public StockType ScoopedType { get; protected set; }

        public float Size { get; protected set; }

        public IObject? Target { get; set; }

        public ShipType Type { get; protected set; }

        public int VanishPoint { get; protected set; }

        public float Velocity { get; set; }

        public float VelocityMax { get; protected set; }
    }
}
