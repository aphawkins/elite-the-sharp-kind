// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Graphics
{
    public readonly struct EColor : IEquatable<EColor>
    {
        public EColor(int argbColour)
        {
            A = 0xFF;
            R = (byte)(argbColour >> 16);
            G = (byte)(argbColour >> 8);
            B = (byte)argbColour;
        }

        public EColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public byte A { get; }

        public byte B { get; }

        public byte G { get; }

        public byte R { get; }

        public static bool operator !=(in EColor left, in EColor right) => !(left == right);

        public static bool operator ==(in EColor left, in EColor right) => left.ToArgb() == right.ToArgb();

        public override bool Equals(object? obj) => obj is EColor other && Equals(other);

        public bool Equals(EColor other) => this == other;

        public override int GetHashCode() => ToArgb().GetHashCode();

        public int ToArgb() => (A << 24) | (R << 16) | (G << 8) | B;
    }
}
