// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Graphics
{
    public readonly struct EColor : IEquatable<EColor>
    {
        private readonly int _argbColour;

        public EColor(int argbColour) => _argbColour = argbColour | unchecked((int)0xFF000000);

        public static EColor Black => new(0x000000);

        public static EColor Blue => new(0x000080);

        public static EColor Cyan => new(0x00FFFF);

        public static EColor DarkBlue => new(0x101090);

        public static EColor DarkerGrey => new(0x606060);

        public static EColor DarkGrey => new(0x707070);

        public static EColor DarkOrange => new(0xE06020);

        public static EColor DarkYellow => new(0xD09010);

        public static EColor Gold => new(0xF0B030);

        public static EColor Green => new(0x008000);

        public static EColor Grey => new(0x727272);

        public static EColor LightBlue => new(0x105090);

        public static EColor LighterGreen => new(0x60E020);

        public static EColor LighterGrey => new(0xE0E0E0);

        public static EColor LighterRed => new(0xC00000);

        public static EColor LightGreen => new(0x80C000);

        public static EColor LightGrey => new(0x808080);

        public static EColor LightOrange => new(0xF0B070);

        public static EColor LightRed => new(0x901010);

        public static EColor LightYellow => new(0xF0F0B0);

        public static EColor Lilac => new(0xE0A0E0);

        public static EColor Orange => new(0xF07030);

        public static EColor Purple => new(0x404080);

        public static EColor Red => new(0x800000);

        public static EColor RedOrange => new(0xF03030);

        public static EColor White => new(0xFFFFFF);

        public static EColor Yellow => new(0xFFFF00);

        public static bool operator !=(EColor left, EColor right) => !(left == right);

        public static bool operator ==(EColor left, EColor right) => left.ToArgb() == right.ToArgb();

        public static IEnumerable<EColor> AllColors() => new List<EColor>()
        {
            Black,
            Blue,
            Cyan,
            DarkBlue,
            DarkerGrey,
            DarkGrey,
            DarkOrange,
            DarkYellow,
            Gold,
            Green,
            Grey,
            LightBlue,
            LighterGreen,
            LighterGrey,
            LighterRed,
            LightGreen,
            LightGrey,
            LightOrange,
            LightRed,
            LightYellow,
            Lilac,
            Orange,
            Purple,
            Red,
            RedOrange,
            White,
            Yellow,
        };

        public override bool Equals(object? obj) => obj is EColor other && Equals(other);

        public bool Equals(EColor other) => this == other;

        public override int GetHashCode() => ToArgb().GetHashCode();

        public int ToArgb() => _argbColour;
    }
}
