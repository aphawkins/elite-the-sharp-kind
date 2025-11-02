// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Suns;

internal static class SunFactory
{
    internal static IObject Create(SunType type, IEliteDraw draw) => type switch
    {
        SunType.Solid => new SolidSun(draw, EliteColors.White),
        SunType.Gradient => new GradientSun(draw),
        _ => throw new EliteException(),
    };
}
