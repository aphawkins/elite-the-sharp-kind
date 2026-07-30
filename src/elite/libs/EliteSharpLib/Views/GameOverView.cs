// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views;

/// <summary>
/// The 16-bit game over screen: the 512-space layout, and nothing else. The
/// wreckage tumbling behind it is drawn by the universe, not here.
/// </summary>
internal sealed class GameOverView : IView<GameOverModel>
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorGold;

    internal GameOverView(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(GameOverModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.Graphics.DrawTextCentre(_draw.Centre.Y, model.Message, nameof(FontType.Large), _colorGold);
    }
}
