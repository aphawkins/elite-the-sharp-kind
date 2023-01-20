namespace Elite.Structs
{
    internal class ship_face_normal
    {
        internal int dist;
        internal int x;
        internal int y;
        internal int z;

        internal ship_face_normal(int dist, int x, int y, int z)
        {
            this.dist = dist;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    };
}