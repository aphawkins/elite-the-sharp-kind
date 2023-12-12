// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Graphics
{
    public readonly struct FastColor : IEquatable<FastColor>
    {
        public FastColor(uint argbColour)
        {
            Argb = argbColour;

            A = (byte)(argbColour >> 24);
            R = (byte)(argbColour >> 16);
            G = (byte)(argbColour >> 8);
            B = (byte)argbColour;
        }

        public FastColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;

            Argb = (uint)((a << 24) | (r << 16) | (g << 8) | b);
        }

        public byte A { get; }

        public uint Argb { get; }

        public byte B { get; }

        public byte G { get; }

        public byte R { get; }

        public static bool operator !=(in FastColor left, in FastColor right) => !(left == right);

        public static bool operator ==(in FastColor left, in FastColor right) => left.Argb == right.Argb;

        public override bool Equals(object? obj) => obj is FastColor other && Equals(other);

        public bool Equals(FastColor other) => this == other;

        public override int GetHashCode() => Argb.GetHashCode();
    }
}
