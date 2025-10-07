// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Useful.Graphics;

public static class BaseColors
{
    public static FastColor TransparentBlack => new(0x00000000);

    public static FastColor TransparentWhite => new(0x00FFFFFF);

    public static FastColor Black => new(0xFF000000);

    public static FastColor White => new(0xFFFFFFFF);

    public static FastColor Red => new(0xFFFF0000);

    public static FastColor Green => new(0xFF00FF00);

    public static FastColor Blue => new(0xFF0000FF);

    public static FastColor Magenta => new(0xFFFF00FF);

    public static FastColor Cyan => new(0xFF00FFFF);

    public static FastColor Yellow => new(0xFFFFFF00);
}
