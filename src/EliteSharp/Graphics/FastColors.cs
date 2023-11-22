// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Graphics
{
    public static class FastColors
    {
        public static FastColor Black => new(0x000000);

        public static FastColor Blue => new(0x000080);

        public static FastColor Cyan => new(0x00FFFF);

        public static FastColor DarkBlue => new(0x101090);

        public static FastColor DarkerGrey => new(0x606060);

        public static FastColor DarkGrey => new(0x707070);

        public static FastColor DarkOrange => new(0xE06020);

        public static FastColor DarkYellow => new(0xD09010);

        public static FastColor Gold => new(0xF0B030);

        public static FastColor Green => new(0x008000);

        public static FastColor Grey => new(0x727272);

        public static FastColor LightBlue => new(0x105090);

        public static FastColor LighterGreen => new(0x60E020);

        public static FastColor LighterGrey => new(0xE0E0E0);

        public static FastColor LighterRed => new(0xC00000);

        public static FastColor LightGreen => new(0x80C000);

        public static FastColor LightGrey => new(0x808080);

        public static FastColor LightOrange => new(0xF0B070);

        public static FastColor LightRed => new(0x901010);

        public static FastColor LightYellow => new(0xF0F0B0);

        public static FastColor Lilac => new(0xE0A0E0);

        public static FastColor Orange => new(0xF07030);

        public static FastColor Purple => new(0x404080);

        public static FastColor Red => new(0x800000);

        public static FastColor RedOrange => new(0xF03030);

        public static FastColor White => new(0xFFFFFF);

        public static FastColor Yellow => new(0xFFFF00);

        public static IEnumerable<FastColor> AllColors() => new List<FastColor>()
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
    }
}
