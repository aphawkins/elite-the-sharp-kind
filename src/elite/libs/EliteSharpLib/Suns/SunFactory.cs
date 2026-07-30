// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Assets;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Suns;

internal static class SunFactory
{
    // As PlanetFactory: a wireframe world overrides the sun style.
    internal static IObject Create(GraphicStyle style, SunType type, IEliteDraw draw, RNG rng)
        => style == GraphicStyle.Wireframe
            ? new WireframeSun(draw)
            : type switch
            {
                SunType.Solid => new SolidSun(draw, rng),
                SunType.Gradient => GradientSun(draw, rng),
                _ => throw new EliteException(),
            };

    private static IObject GradientSun(IEliteDraw draw, RNG rng)
        => draw.Tier == SystemTier.EightBit ? new GradientSun8Bit(draw, rng) : new GradientSun16Bit(draw, rng);
}
