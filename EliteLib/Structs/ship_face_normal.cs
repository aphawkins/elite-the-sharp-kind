namespace Elite.Structs
{
    using System.Numerics;

    internal class ship_face_normal
    {
        internal int dist;
        internal Vector3 direction;

        internal ship_face_normal(int dist, Vector3 direction)
        {
            this.dist = dist;
            this.direction = direction;
        }
    };
}