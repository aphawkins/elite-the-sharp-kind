// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Stars;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The starfield in this rendition's white, with the original's threshold for
/// a star being near enough to draw a pixel wider.
/// </summary>
internal sealed class StarfieldRenderer16Bit : StarfieldRendererBase
{
    internal StarfieldRenderer16Bit(IViewSurface surface)
        : base(surface)
        => Colour = surface.Palette["White"];

    protected override FastColor Colour { get; }

    protected override float WideDistance => 192;

    protected override float BlockDistance => 144;
}
