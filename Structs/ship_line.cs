namespace Elite.Structs
{
    public struct ship_line
    {
        public int dist;
        public int face1;
        public int face2;
        public int start_point;
        public int end_point;

        public ship_line(int dist, int face1, int face2, int start_point, int end_point)
        {
            this.dist = dist;
            this.face1 = face1;
            this.face2 = face2;
            this.start_point = start_point;
            this.end_point = end_point;
        }
    };
}