// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Lasers;

internal sealed class LaserDraw8Bit : LaserDrawBase
{
    private readonly FastColor _colorPurple;
    private readonly FastColor _colorRed;
    private readonly FastColor _colorYellow;

    internal LaserDraw8Bit(GameState gameState, IEliteDraw draw, RNG rng)
        : base(gameState, draw, rng)
    {
        ArgumentNullException.ThrowIfNull(draw);

        _colorPurple = draw.Palette["Purple"];
        _colorRed = draw.Palette["Red"];
        _colorYellow = draw.Palette["Yellow"];
    }

    // Beam and mining match their crosshair sprite's shade - laser-mining.bmp
    // is painted Purple - and pulse and military share the default.
    protected override FastColor BeamColor(LaserType laserType) => laserType switch
    {
        LaserType.Beam => _colorYellow,
        LaserType.Mining => _colorPurple,
        _ => _colorRed,
    };
}
