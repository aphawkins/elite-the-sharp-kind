// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Graphics;

namespace EliteSharpLib.Graphics;

public static class EliteColors
{
    public static FastColor Black => new(0xFF000000);

    public static FastColor Blue => new(0xFF000080);

    public static FastColor Cyan => new(0xFF00FFFF);

    public static FastColor DarkBlue => new(0xFF101090);

    public static FastColor DarkerGrey => new(0xFF606060);

    public static FastColor DarkGrey => new(0xFF707070);

    public static FastColor DarkOrange => new(0xFFE06020);

    public static FastColor DarkYellow => new(0xFFD09010);

    public static FastColor Gold => new(0xFFF0B030);

    public static FastColor Green => new(0xFF008000);

    public static FastColor Grey => new(0xFF727272);

    public static FastColor LightBlue => new(0xFF105090);

    public static FastColor LighterGreen => new(0xFF60E020);

    public static FastColor LighterGrey => new(0xFFE0E0E0);

    public static FastColor LighterRed => new(0xFFC00000);

    public static FastColor LightGreen => new(0xFF80C000);

    public static FastColor LightGrey => new(0xFF808080);

    public static FastColor LightOrange => new(0xFFF0B070);

    public static FastColor LightRed => new(0xFF901010);

    public static FastColor LightYellow => new(0xFFF0F0B0);

    public static FastColor Lilac => new(0xFFE0A0E0);

    public static FastColor Orange => new(0xFFF07030);

    public static FastColor Purple => new(0xFF404080);

    public static FastColor Red => new(0xFF800000);

    public static FastColor RedOrange => new(0xFFF03030);

    public static FastColor White => new(0xFFFFFFFF);

    public static FastColor Yellow => new(0xFFFFFF00);

    public static IEnumerable<FastColor> AllColors
    => [
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
    ];
}
