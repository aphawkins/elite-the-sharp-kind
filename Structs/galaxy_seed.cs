namespace Elite.Structs
{
    internal struct galaxy_seed
    {
        internal int a;    /* 6c */
        internal int b;    /* 6d */
        internal int c;    /* 6e */
        internal int d;    /* 6f */
        internal int e;    /* 70 */
        internal int f;    /* 71 */

        internal galaxy_seed(int a, int b, int c, int d, int e, int f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
        }
    };
}