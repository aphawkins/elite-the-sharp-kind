using System.Numerics;

namespace Elite.Engine.Types
{
    internal class ShipFaceNormal
    {
        internal int dist;
        internal Vector3 direction;

        internal ShipFaceNormal(int dist, Vector3 direction)
        {
            this.dist = dist;
            this.direction = direction;
        }
    };
}