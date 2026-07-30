// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Lasers;

internal sealed class LaserDraw16Bit : LaserDrawBase
{
    private readonly uint _colorMediumOrchid;
    private readonly uint _colorKhaki;
    private readonly uint _colorCrimson;

    internal LaserDraw16Bit(GameState gameState, IEliteDraw draw, RNG rng)
        : base(gameState, draw, rng)
    {
        _colorKhaki = draw.Palette["Khaki"];
        _colorCrimson = draw.Palette["Crimson"];
        _colorMediumOrchid = draw.Palette["MediumOrchid"];
    }

    protected override uint BeamColor(LaserType laserType) => laserType switch
    {
        LaserType.Beam => _colorKhaki,
        LaserType.Mining => _colorMediumOrchid,
        _ => _colorCrimson,
    };
}
