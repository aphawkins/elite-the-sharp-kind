// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The flat-disc planet, drawn in this rendition's green.
/// </summary>
internal sealed class SolidPlanetRenderer16Bit : SolidPlanetRendererBase
{
    internal SolidPlanetRenderer16Bit(IViewSurface surface)
        : base(surface)
        => Colour = surface.Palette["Green"];

    protected override FastColor Colour { get; }
}
