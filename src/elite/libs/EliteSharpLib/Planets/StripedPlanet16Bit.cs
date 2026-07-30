// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Planets;

internal sealed class StripedPlanet16Bit : StripedPlanetBase
{
    internal StripedPlanet16Bit(IEliteDraw draw)
        : base(draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        uint colorDarkSlateBlue = draw.Palette["DarkSlateBlue"];
        uint colorDarkBlue = draw.Palette["DarkBlue"];
        uint colorNavy = draw.Palette["Navy"];
        uint colorTeal = draw.Palette["Teal"];
        uint colorGainsboro = draw.Palette["Gainsboro"];
        uint colorTomato = draw.Palette["Tomato"];
        uint colorSandyBrown = draw.Palette["SandyBrown"];
        uint colorChocolate = draw.Palette["Chocolate"];
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

    protected override uint[] StripeColors { get; }

    public override IObject Clone()
    {
        StripedPlanet16Bit planet = new(this);
        this.CopyTo(planet);
        return planet;
    }
}
