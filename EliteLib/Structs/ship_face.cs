namespace Elite.Structs
{
    using System.Numerics;
    using Elite.Enums;

    internal class ship_face
    {
        internal GFX_COL colour;
        internal Vector3 normal;
        internal int[] points;

        internal ship_face(GFX_COL colour, Vector3 normal, int[] points)
        {
            this.colour = colour;
            this.normal = normal;
            this.points = points;
        }
    };
}
