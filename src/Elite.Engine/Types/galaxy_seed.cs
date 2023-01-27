namespace Elite.Engine.Types
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