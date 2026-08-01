// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Lasers;

internal sealed class LaserDraw16Bit : LaserDrawBase
{
    private readonly FastColor _colorBrightPurple;
    private readonly FastColor _colorPaleYellow;
    private readonly FastColor _colorRedOrange;

    internal LaserDraw16Bit(GameState gameState, IEliteDraw draw, RNG rng)
        : base(gameState, draw, rng)
    {
        _colorPaleYellow = draw.Palette["PaleYellow"];
        _colorRedOrange = draw.Palette["RedOrange"];
        _colorBrightPurple = draw.Palette["BrightPurple"];
    }

    protected override FastColor BeamColor(LaserType laserType) => laserType switch
    {
        LaserType.Beam => _colorPaleYellow,
        LaserType.Mining => _colorBrightPurple,
        _ => _colorRedOrange,
    };
}
