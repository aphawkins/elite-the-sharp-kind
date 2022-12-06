namespace Elite.Structs
{
    public struct ship_point
    {
        public int x;
        public int y;
        public int z;
        public int dist;
        public int face1;
        public int face2;
        public int face3;
        public int face4;

        public ship_point(int x, int y, int z, int dist, int face1, int face2, int face3, int face4)
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