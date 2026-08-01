// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib.Planets;

internal sealed class StripedPlanet16Bit : StripedPlanetBase
{
    internal StripedPlanet16Bit(IEliteDraw draw)
        : base(draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        FastColor colorPurple = draw.Palette["Purple"];
        FastColor colorDarkBlue = draw.Palette["DarkBlue"];
        FastColor colorBlue = draw.Palette["Blue"];
        FastColor colorLightBlue = draw.Palette["LightBlue"];
        FastColor colorLighterGrey = draw.Palette["LighterGrey"];
        FastColor colorOrange = draw.Palette["Orange"];
        FastColor colorLightOrange = draw.Palette["LightOrange"];
        FastColor colorDarkOrange = draw.Palette["DarkOrange"];
        StripeColors =
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

        GenerateLandscape();
    }

    private StripedPlanet16Bit(StripedPlanet16Bit other)
        : base(other)
        => StripeColors = other.StripeColors;

    protected override FastColor[] StripeColors { get; }

    public override IObject Clone()
    {
        StripedPlanet16Bit planet = new(this);
        this.CopyTo(planet);
        return planet;
    }
}
