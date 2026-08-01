// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit game over screen: the 512-space layout, and nothing else. The
/// wreckage tumbling behind it is drawn by the universe, not here.
/// </summary>
internal sealed class GameOverView16Bit : BaseView16Bit, IView<GameOverModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGold;

    internal GameOverView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(GameOverModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        _draw.Graphics.DrawTextCentre(_draw.Layout.ViewportCentre.Y, model.Message, nameof(FontType.Large), _colorGold);
    }
}
