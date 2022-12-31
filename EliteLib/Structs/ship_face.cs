namespace Elite.Structs
{
    using Elite.Enums;

    internal struct ship_face
    {
        internal GFX_COL colour;
        internal int norm_x;
        internal int norm_y;
        internal int norm_z;
        internal int points;
        internal int p1;
        internal int p2;
        internal int p3;
        internal int p4;
        internal int p5;
        internal int p6;
        internal int p7;
        internal int p8;

        internal ship_face(GFX_COL colour, int norm_x, int norm_y, int norm_z, int points, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8)
        {
            this.colour = colour;
            this.norm_x = norm_x;
            this.norm_y = norm_y;
            this.norm_z = norm_z;
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
