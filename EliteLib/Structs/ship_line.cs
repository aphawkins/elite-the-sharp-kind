﻿namespace Elite.Structs
{
    internal struct ship_line
    {
        internal int dist;
        internal int face1;
        internal int face2;
        internal int start_point;
        internal int end_point;

        internal ship_line(int dist, int face1, int face2, int start_point, int end_point)
        {
            this.dist = dist;
            this.face1 = face1;
            this.face2 = face2;
            this.start_point = start_point;
            this.end_point = end_point;
        }
    };
}