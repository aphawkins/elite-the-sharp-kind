namespace Elite.Structs
{
    using System.Numerics;

    internal class ship_point
    {
        internal Vector3 point;
        internal int dist;
        internal int face1;
        internal int face2;
        internal int face3;
        internal int face4;

        internal ship_point(Vector3 point, int dist, int face1, int face2, int face3, int face4)
        {
            this.point = point;
            this.dist = dist;
            this.face1 = face1;
            this.face2 = face2;
            this.face3 = face3;
            this.face4 = face4;
        }
    };
}