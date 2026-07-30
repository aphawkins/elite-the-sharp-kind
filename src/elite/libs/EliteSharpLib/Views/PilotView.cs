// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Lasers;

namespace EliteSharpLib.Views;

/// <summary>
/// The cockpit window: shared by both tiers and all four directions, since
/// none varies the layout, only the model's content. The hyperspace status
/// line was the one absolute coordinate here (a hardcoded y=358, which only
/// happened to work because it equalled the 16-bit ScannerTop minus 25);
/// deriving it from ScannerTop instead means this needs no 8-bit
/// counterpart at all - see docs/backlog-roadmap.md's per-tier view survey.
/// The starfield and the ship ahead are drawn by the universe.
/// </summary>
internal sealed class PilotView : IView<PilotModel>
{
    private readonly IEliteDraw _draw;
    private readonly LaserDraw _laser;
    private readonly uint _colorWhite;

    internal PilotView(IEliteDraw draw, GameState gameState, RNG rng)
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
            _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 25, model.HyperspaceStatus, nameof(FontType.Small), _colorWhite);
        }

        _draw.Graphics.DrawTextCentre(_draw.Top + 10, model.ViewName, nameof(FontType.Small), _colorWhite);

        if (model.IsFiring)
        {
            _laser.DrawLaserLines(model.LaserType);
        }

        _laser.DrawLaserSights(model.LaserType);
    }
}
