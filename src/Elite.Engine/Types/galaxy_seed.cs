// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Types
{
    public class GalaxySeed : ICloneable
    {
        public int A { get; set; }    /* 6c */
        public int B { get; set; }    /* 6d */
        public int C { get; set; }    /* 6e */
        public int D { get; set; }    /* 6f */
        public int E { get; set; }    /* 70 */
        public int F { get; set; }    /* 71 */

        public GalaxySeed()
        {
        }

        protected GalaxySeed(GalaxySeed other)
        {
            A = other.A;
            B = other.B;
            C = other.C;
            D = other.D;
            E = other.E;
            F = other.F;
        }

        public object Clone() => new GalaxySeed(this);
    }
}