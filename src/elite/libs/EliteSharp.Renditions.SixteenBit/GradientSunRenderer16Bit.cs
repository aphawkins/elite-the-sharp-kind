// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Suns;
using Useful;
using Useful.Maths;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The banded sun: white at the core, then yellow and orange rings, with the
/// outermost band dithered between two oranges this rendition's ramp has and
/// the 8-bit one does not.
/// </summary>
internal sealed class GradientSunRenderer16Bit : GradientSunRendererBase
{
    private readonly FastColor _darkOrange;
    private readonly FastColor _lightOrange;
    private readonly FastColor _lightYellow;
    private readonly FastColor _orange;
    private readonly FastColor _white;

    internal GradientSunRenderer16Bit(IViewSurface surface, IRandomSource random)
        : base(surface, random)
    {
        _white = surface.Palette["White"];
        _lightYellow = surface.Palette["LightYellow"];
        _lightOrange = surface.Palette["LightOrange"];
        _orange = surface.Palette["Orange"];
        _darkOrange = surface.Palette["DarkOrange"];
    }

    protected override FastColor SunColor(float distance, float inner, float inner2, float outer, int dither)
        => distance < inner
            ? _white
            : distance < inner2
                ? _lightYellow
                : distance < outer
                    ? _lightOrange
                    : dither.IsOdd() ? _orange : _darkOrange;
}
