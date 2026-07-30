// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Maths;

namespace EliteSharpLib.Suns;

internal sealed class GradientSun16Bit : GradientSunBase
{
    private readonly uint _colorChocolate;
    private readonly uint _colorSandyBrown;
    private readonly uint _colorPaleGoldenrod;
    private readonly uint _colorTomato;
    private readonly uint _colorWhite;

    internal GradientSun16Bit(IEliteDraw draw, RNG rng)
        : base(draw, rng)
    {
        ArgumentNullException.ThrowIfNull(draw);

        _colorWhite = draw.Palette["White"];
        _colorPaleGoldenrod = draw.Palette["PaleGoldenrod"];
        _colorSandyBrown = draw.Palette["SandyBrown"];
        _colorTomato = draw.Palette["Tomato"];
        _colorChocolate = draw.Palette["Chocolate"];
    }

    private GradientSun16Bit(GradientSun16Bit other)
        : base(other)
    {
        _colorWhite = other._colorWhite;
        _colorPaleGoldenrod = other._colorPaleGoldenrod;
        _colorSandyBrown = other._colorSandyBrown;
        _colorTomato = other._colorTomato;
        _colorChocolate = other._colorChocolate;
    }

    public override IObject Clone()
    {
        GradientSun16Bit sun = new(this);
        this.CopyTo(sun);
        return sun;
    }

    // The sun's banding: white at the core, then yellow and orange rings, with
    // the outermost band dithered between two oranges.
    protected override uint SunColor(float distance, float inner, float inner2, float outer, int dither)
        => distance < inner
            ? _colorWhite
            : distance < inner2
                ? _colorPaleGoldenrod
                : distance < outer
                    ? _colorSandyBrown
                    : dither.IsOdd() ? _colorTomato : _colorChocolate;
}
