namespace Elite.Structs
{
    using Elite.Enums;
    using EliteLib;

    internal struct univ_object
    {
        internal int type;
        internal Vector location;
        internal Matrix rotmat;
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
