// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Suns;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The solid-world sun, drawn in this rendition's white.
/// </summary>
internal sealed class SolidSunRenderer16Bit : SolidSunRendererBase
{
    internal SolidSunRenderer16Bit(IViewSurface surface, IRandomSource random)
        : base(surface, random)
        => Colour = surface.Palette["White"];

    protected override FastColor Colour { get; }
}
