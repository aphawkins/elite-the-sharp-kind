// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Lasers;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit cockpit window: shared by all four directions, since none varies
/// the layout, only the model's content. Only the two text rows are tiered -
/// the crosshair and beams that <see cref="LaserDraw"/> renders are entirely
/// Centre/Scale-relative and need no 8-bit variant. The starfield and the
/// ship ahead are drawn by the universe.
/// </summary>
internal sealed class PilotView8Bit : IView<PilotModel>
{
    private const float HyperspaceOffsetY = 12;

    private readonly IEliteDraw _draw;
    private readonly LaserDraw _laser;
    private readonly uint _colorWhite;

    internal PilotView8Bit(IEliteDraw draw, GameState gameState, RNG rng)
    {
        _draw = draw;
        _laser = new LaserDraw(gameState, draw, rng);

        _colorWhite = draw.Palette["White"];
    }

    public void Draw(PilotModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.HyperspaceStatus.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(
                _draw.ScannerTop - HyperspaceOffsetY,
                model.HyperspaceStatus,
                nameof(FontType.Small),
                _colorWhite);
        }

        _draw.Graphics.DrawTextCentre(_draw.Top + 4, model.ViewName, nameof(FontType.Small), _colorWhite);

        if (model.IsFiring)
        {
            _laser.DrawLaserLines(model.LaserType);
        }

        _laser.DrawLaserSights(model.LaserType);
    }
}
