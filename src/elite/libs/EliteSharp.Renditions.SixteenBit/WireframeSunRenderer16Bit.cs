// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Suns;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The wireframe-world sun, drawn in this rendition's white.
/// </summary>
internal sealed class WireframeSunRenderer16Bit : WireframeSunRendererBase
{
    internal WireframeSunRenderer16Bit(IViewSurface surface)
        : base(surface)
        => Colour = surface.Palette["White"];

    protected override FastColor Colour { get; }
}
