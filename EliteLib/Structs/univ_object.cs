namespace Elite.Structs
{
    using System.Numerics;
    using Elite.Enums;

    internal class univ_object : ICloneable
    {
        internal SHIP type;
        internal Vector3 location;
        internal Vector3[] rotmat;
        internal int rotx;
        internal int rotz;
        internal FLG flags;
        internal int energy;
        internal float velocity;
        internal int acceleration;
        internal int missiles;
        internal int target;
        internal int bravery;
        internal int exp_delta;
        internal int exp_seed;
        internal float distance;

        internal univ_object()
        {
        }

        private univ_object(univ_object other)
        {
            type = other.type;
            location = other.location.Cloner();
            rotmat = other.rotmat.Cloner();
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
            exp_seed = other.exp_seed;
            distance = other.distance;
        }

        public object Clone()
        {
            return new univ_object(this);
        }
    }
}