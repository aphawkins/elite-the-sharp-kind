// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Ships;

namespace Elite.Engine.Types
{
    internal sealed class UniverseObject
    {
        internal UniverseObject()
        {
        }

        internal UniverseObject(UniverseObject other)
        {
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

        internal int Acceleration { get; set; }

        internal int Bravery { get; set; }

        internal int Energy { get; set; }

        internal int ExpDelta { get; set; }

        internal ShipFlags Flags { get; set; }

        internal Vector3 Location { get; set; }

        internal int Missiles { get; set; }

        internal Vector3[] Rotmat { get; set; } = new Vector3[3];

        internal float RotX { get; set; }

        internal float RotZ { get; set; }

        internal int Target { get; set; }

        internal ShipType Type { get; set; }

        internal float Velocity { get; set; }
    }
}
