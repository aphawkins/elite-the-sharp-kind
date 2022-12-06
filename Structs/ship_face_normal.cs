namespace Elite.Structs
{
    public struct ship_face_normal
    {
        public int dist;
        public int x;
        public int y;
        public int z;

        public ship_face_normal(int dist, int x, int y, int z)
        {
            this.dist = dist;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    };
}