// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Types
{
    internal sealed class GalaxySeed
    {
        internal GalaxySeed()
        {
        }

        internal GalaxySeed(GalaxySeed other)
        {
            A = other.A;
            B = other.B;
            C = other.C;
            D = other.D;
            E = other.E;
            F = other.F;
        }

        // 6c
        internal int A { get; set; }

        // 6d
        internal int B { get; set; }

        // 6e
        internal int C { get; set; }

        // 6f
        internal int D { get; set; }

        // 70
        internal int E { get; set; }

        // 71
        internal int F { get; set; }
    }
}
