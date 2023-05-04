using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.Engine.Types
{
    internal class UniverseObject
    {
        internal ShipType type;
        internal Vector3 location;
        internal Vector3[] Rotmat = new Vector3[3];
        internal float rotx;
        internal float rotz;
        internal FLG flags;
        internal int energy;
        internal float velocity;
        internal int acceleration;
        internal int missiles;
        internal int target;
        internal int bravery;
        internal int exp_delta;

        internal UniverseObject()
        {
        }

        internal UniverseObject(UniverseObject other)
        {
            type = other.type;
            location = other.location.Cloner();
            Rotmat = other.Rotmat.Cloner();
            rotx = other.rotx;
            rotz = other.rotz;
            flags = other.flags;
            energy = other.energy;
            velocity = other.velocity;
            acceleration = other.acceleration;
            missiles = other.missiles;
            target = other.target;
            bravery = other.bravery;
            exp_delta = other.exp_delta;
        }
    }
}