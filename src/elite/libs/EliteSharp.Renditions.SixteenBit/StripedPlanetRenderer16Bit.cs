// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

internal sealed class StripedPlanetRenderer16Bit : StripedPlanetRendererBase
{
    internal StripedPlanetRenderer16Bit(IViewSurface surface)
        : base(surface)
    {
        FastColor colorPurple = surface.Palette["Purple"];
        FastColor colorDarkBlue = surface.Palette["DarkBlue"];
        FastColor colorBlue = surface.Palette["Blue"];
        FastColor colorLightBlue = surface.Palette["LightBlue"];
        FastColor colorLighterGrey = surface.Palette["LighterGrey"];
        FastColor colorOrange = surface.Palette["Orange"];
        FastColor colorLightOrange = surface.Palette["LightOrange"];
        FastColor colorDarkOrange = surface.Palette["DarkOrange"];
        Stripes =
        [
            colorPurple,
            colorPurple,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorLightBlue,
            colorLightBlue,
            colorLighterGrey,
            colorOrange,
            colorOrange,
            colorOrange,
            colorOrange,
            colorLightOrange,
            colorOrange,
            colorOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorOrange,
            colorLightOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorOrange,
            colorOrange,
            colorLightOrange,
            colorOrange,
            colorOrange,
            colorOrange,
            colorOrange,
            colorLighterGrey,
            colorLightBlue,
            colorLightBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorPurple,
            colorPurple,
        ];
    }

    protected override IReadOnlyList<FastColor> Stripes { get; }
}
