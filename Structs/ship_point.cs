namespace Elite.Structs
{
    internal struct ship_point
    {
        internal int x;
        internal int y;
        internal int z;
        internal int dist;
        internal int face1;
        internal int face2;
        internal int face3;
        internal int face4;

        internal ship_point(int x, int y, int z, int dist, int face1, int face2, int face3, int face4)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.dist = dist;
            this.face1 = face1;
            this.face2 = face2;
            this.face3 = face3;
            this.face4 = face4;
        }
    };
}