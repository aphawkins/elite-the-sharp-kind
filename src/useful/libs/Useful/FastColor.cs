// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful;

public readonly struct FastColor : IEquatable<FastColor>
{
    public FastColor(uint argbColor)
    {
        Argb = argbColor;

        A = (byte)(argbColor >> 24);
        R = (byte)(argbColor >> 16);
        G = (byte)(argbColor >> 8);
        B = (byte)argbColor;
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

    // Eases the uint-to-FastColor migration: ARGB literals/fields keep
    // working across the boundary while call sites move to FastColor.
    public static implicit operator FastColor(uint argbColor) => FromUInt32(argbColor);

    public static implicit operator uint(in FastColor color) => ToUInt32(color);

    public static bool operator !=(in FastColor left, in FastColor right) => !(left == right);

    public static bool operator ==(in FastColor left, in FastColor right) => left.Argb == right.Argb;

    public static FastColor FromUInt32(uint argbColor) => new(argbColor);

    public static uint ToUInt32(in FastColor color) => color.Argb;

    public override bool Equals(object? obj) => obj is FastColor other && Equals(other);

    public bool Equals(FastColor other) => this == other;

    public override int GetHashCode() => Argb.GetHashCode();

    public override string ToString() => $"0x{Argb:X}";
}
