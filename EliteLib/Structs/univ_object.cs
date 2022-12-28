namespace Elite.Structs
{
    using Elite.Enums;

    internal struct univ_object
    {
        internal SHIP type;
        internal Vector location;
        internal Vector[] rotmat;
        internal int rotx;
        internal int rotz;
        internal FLG flags;
        internal int energy;
        internal int velocity;
        internal int acceleration;
        internal int missiles;
        internal int target;
        internal int bravery;
        internal int exp_delta;
        internal int exp_seed;
        internal int distance;
    };
}