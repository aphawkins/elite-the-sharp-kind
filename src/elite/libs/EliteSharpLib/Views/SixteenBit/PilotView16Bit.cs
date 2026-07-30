// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Lasers;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit cockpit window: shared by all four directions, since none
/// varies the layout, only the model's content. The hyperspace status line
/// used to be an absolute y=358, which only worked because it equalled
/// ScannerTop minus 25; it is derived now, so the only thing separating this
/// from PilotView8Bit is the two offsets. The starfield and the ship ahead are
/// drawn by the universe.
/// </summary>
internal sealed class PilotView16Bit : BaseView16Bit, IView<PilotModel>
{
    private readonly IEliteDraw _draw;
    private readonly LaserDrawBase _laser;
    private readonly FastColor _colorWhite;

    internal PilotView16Bit(IEliteDraw draw, GameState gameState, RNG rng)
        : base(draw)
    {
        _draw = draw;
        _laser = new LaserDraw16Bit(gameState, draw, rng);

        _colorWhite = draw.Palette["White"];
    }

    public void Draw(PilotModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.HyperspaceStatus.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(_draw.Layout.ScannerTop - 25, model.HyperspaceStatus, nameof(FontType.Small), _colorWhite);
        }

        _draw.Graphics.DrawTextCentre(_draw.Layout.Top + 10, model.ViewName, nameof(FontType.Small), _colorWhite);

        if (model.IsFiring)
        {
            _laser.DrawLaserLines(model.LaserType);
        }

        _laser.DrawLaserSights(model.LaserType);
    }
}
