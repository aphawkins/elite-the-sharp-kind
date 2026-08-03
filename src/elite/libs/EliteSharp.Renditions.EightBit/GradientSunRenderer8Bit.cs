// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Suns;
using Useful;
using Useful.Maths;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The banded sun: white, yellow, orange, brown - the warm end of this
/// rendition's sixteen colours, since it has no orange ramp to grade through
/// the way the 16-bit one does.
/// </summary>
internal sealed class GradientSunRenderer8Bit : GradientSunRendererBase
{
    private readonly FastColor _brown;
    private readonly FastColor _orange;
    private readonly FastColor _white;
    private readonly FastColor _yellow;

    internal GradientSunRenderer8Bit(IViewSurface surface, IRandomSource random)
        : base(surface, random)
    {
        _white = surface.Palette["White"];
        _yellow = surface.Palette["Yellow"];
        _orange = surface.Palette["Orange"];
        _brown = surface.Palette["Brown"];
    }

    protected override FastColor SunColor(float distance, float inner, float inner2, float outer, int dither)
        => distance < inner
            ? _white
            : distance < inner2
                ? _yellow
                : distance < outer
                    ? _orange
                    : dither.IsOdd() ? _orange : _brown;
}
