// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Ships;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

internal sealed class LaserDraw16Bit : LaserDrawBase
{
    private readonly FastColor _colorBrightPurple;
    private readonly FastColor _colorPaleYellow;
    private readonly FastColor _colorRedOrange;

    internal LaserDraw16Bit(IViewSurface surface)
        : base(surface)
    {
        _colorPaleYellow = surface.Palette["PaleYellow"];
        _colorRedOrange = surface.Palette["RedOrange"];
        _colorBrightPurple = surface.Palette["BrightPurple"];
    }

    protected override FastColor BeamColor(LaserType laserType) => laserType switch
    {
        LaserType.Beam => _colorPaleYellow,
        LaserType.Mining => _colorBrightPurple,
        _ => _colorRedOrange,
    };
}
