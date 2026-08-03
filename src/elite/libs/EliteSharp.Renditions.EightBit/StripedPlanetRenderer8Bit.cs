// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;
using Useful;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The 8-bit banded planet. The 16-bit map grades through four blues and
/// three oranges; the tier's sixteen colours have neither ramp, so the bands
/// here are blocked rather than graded - blue poles into a brown and orange
/// equator, which is what the hardware this stands in for could show.
/// </summary>
internal sealed class StripedPlanetRenderer8Bit : StripedPlanetRendererBase
{
    internal StripedPlanetRenderer8Bit(IViewSurface surface)
        : base(surface)
    {
        FastColor colorBlue = surface.Palette["Blue"];
        FastColor colorLightBlue = surface.Palette["LightBlue"];
        FastColor colorLightGray = surface.Palette["LightGray"];
        FastColor colorOrange = surface.Palette["Orange"];
        FastColor colorBrown = surface.Palette["Brown"];
        Stripes =
        [
            colorBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorLightBlue,
            colorLightBlue,
            colorLightGray,
            colorOrange,
            colorOrange,
            colorBrown,
            colorBrown,
            colorBrown,
            colorOrange,
            colorOrange,
            colorLightGray,
            colorLightBlue,
            colorLightBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorBlue,
        ];
    }

    protected override IReadOnlyList<FastColor> Stripes { get; }
}
