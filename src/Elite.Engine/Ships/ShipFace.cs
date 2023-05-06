using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class ShipFace
    {
        internal GFX_COL colour;
        internal Vector3 normal;
        internal int[] points;

        internal ShipFace(GFX_COL colour, Vector3 normal, int[] points)
        {
            this.colour = colour;
            this.normal = normal;
            this.points = points;
        }
    };
}
