// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;
using Useful;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The outlined planet, drawn in this rendition's white.
/// </summary>
internal sealed class WireframePlanetRenderer8Bit : WireframePlanetRendererBase
{
    internal WireframePlanetRenderer8Bit(IViewSurface surface, bool hasCrater)
        : base(surface, hasCrater)
        => Colour = surface.Palette["White"];

    protected override FastColor Colour { get; }
}
