namespace Elite.Structs
{
    using System.Numerics;
    using Elite.Enums;

    internal class ship_face
    {
        internal GFX_COL colour;
        internal Vector3 normal;
        internal int points;
        internal int p1;
        internal int p2;
        internal int p3;
        internal int p4;
        internal int p5;
        internal int p6;
        internal int p7;
        internal int p8;

        internal ship_face(GFX_COL colour, Vector3 normal, int points, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8)
        {
            this.colour = colour;
            this.normal = normal;
            this.points = points;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.p5 = p5;
            this.p6 = p6;
            this.p7 = p7;
            this.p8 = p8;
        }
    };
}
