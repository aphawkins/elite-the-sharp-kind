namespace Elite.Structs
{
    public class galaxy_seed : ICloneable
    {
        public int a { get; set; }    /* 6c */
        public int b { get; set; }    /* 6d */
        public int c { get; set; }    /* 6e */
        public int d { get; set; }    /* 6f */
        public int e { get; set; }    /* 70 */
        public int f { get; set; }    /* 71 */

        public galaxy_seed()
        {
            a = 0x4a;
            b = 0x5a;
            c = 0x48;
            d = 0x02;
            e = 0x53;
            f = 0xb7;
        }

        public galaxy_seed(int a, int b, int c, int d, int e, int f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
        }

        protected galaxy_seed(galaxy_seed other)
        {
            a = other.a;
            b = other.b;
            c = other.c;
            d = other.d;
            e = other.e;
            f = other.f;
        }

        public object Clone()
        {
            return new galaxy_seed(this);
        }
    }
}