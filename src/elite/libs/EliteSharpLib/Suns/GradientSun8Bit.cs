// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Maths;

namespace EliteSharpLib.Suns;

/// <summary>
/// The 8-bit sun. Its four bands run white, yellow, orange, brown - the
/// warm end of the tier's sixteen colours, since it has no orange ramp to
/// grade through the way the 16-bit palette does.
/// </summary>
internal sealed class GradientSun8Bit : GradientSunBase
{
    private readonly uint _colorBrown;
    private readonly uint _colorOrange;
    private readonly uint _colorWhite;
    private readonly uint _colorYellow;

    internal GradientSun8Bit(IEliteDraw draw, RNG rng)
        : base(draw, rng)
    {
        ArgumentNullException.ThrowIfNull(draw);

        _colorWhite = draw.Palette["White"];
        _colorYellow = draw.Palette["Yellow"];
        _colorOrange = draw.Palette["Orange"];
        _colorBrown = draw.Palette["Brown"];
    }

    private GradientSun8Bit(GradientSun8Bit other)
        : base(other)
    {
        _colorWhite = other._colorWhite;
        _colorYellow = other._colorYellow;
        _colorOrange = other._colorOrange;
        _colorBrown = other._colorBrown;
    }

    public override IObject Clone()
    {
        GradientSun8Bit sun = new(this);
        this.CopyTo(sun);
        return sun;
    }

    protected override uint SunColor(float distance, float inner, float inner2, float outer, int dither)
        => distance < inner
            ? _colorWhite
            : distance < inner2
                ? _colorYellow
                : distance < outer
                    ? _colorOrange
                    : dither.IsOdd() ? _colorOrange : _colorBrown;
}
