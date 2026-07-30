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

        FastColor colorDarkSlateBlue = draw.Palette["DarkSlateBlue"];
        FastColor colorDarkBlue = draw.Palette["DarkBlue"];
        FastColor colorNavy = draw.Palette["Navy"];
        FastColor colorTeal = draw.Palette["Teal"];
        FastColor colorGainsboro = draw.Palette["Gainsboro"];
        FastColor colorTomato = draw.Palette["Tomato"];
        FastColor colorSandyBrown = draw.Palette["SandyBrown"];
        FastColor colorChocolate = draw.Palette["Chocolate"];
        StripeColors =
        [
            colorDarkSlateBlue,
            colorDarkSlateBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorNavy,
            colorNavy,
            colorNavy,
            colorNavy,
            colorTeal,
            colorTeal,
            colorGainsboro,
            colorTomato,
            colorTomato,
            colorTomato,
            colorTomato,
            colorSandyBrown,
            colorTomato,
            colorTomato,
            colorChocolate,
            colorChocolate,
            colorChocolate,
            colorChocolate,
            colorTomato,
            colorSandyBrown,
            colorChocolate,
            colorChocolate,
            colorChocolate,
            colorChocolate,
            colorChocolate,
            colorChocolate,
            colorTomato,
            colorTomato,
            colorSandyBrown,
            colorTomato,
            colorTomato,
            colorTomato,
            colorTomato,
            colorGainsboro,
            colorTeal,
            colorTeal,
            colorNavy,
            colorNavy,
            colorNavy,
            colorNavy,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkSlateBlue,
            colorDarkSlateBlue,
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
