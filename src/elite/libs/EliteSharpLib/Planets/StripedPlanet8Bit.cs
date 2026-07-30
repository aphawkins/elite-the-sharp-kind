// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib.Planets;

/// <summary>
/// The 8-bit banded planet. The 16-bit map grades through four blues and
/// three oranges; the tier's sixteen colours have neither ramp, so the bands
/// here are blocked rather than graded - blue poles into a brown and orange
/// equator, which is what the hardware this stands in for could show.
/// </summary>
internal sealed class StripedPlanet8Bit : StripedPlanetBase
{
    internal StripedPlanet8Bit(IEliteDraw draw)
        : base(draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        FastColor colorBlue = draw.Palette["Blue"];
        FastColor colorLightBlue = draw.Palette["LightBlue"];
        FastColor colorLightGray = draw.Palette["LightGray"];
        FastColor colorOrange = draw.Palette["Orange"];
        FastColor colorBrown = draw.Palette["Brown"];
        StripeColors =
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

        GenerateLandscape();
    }

    private StripedPlanet8Bit(StripedPlanet8Bit other)
        : base(other)
        => StripeColors = other.StripeColors;

    protected override FastColor[] StripeColors { get; }

    public override IObject Clone()
    {
        StripedPlanet8Bit planet = new(this);
        this.CopyTo(planet);
        return planet;
    }
}
