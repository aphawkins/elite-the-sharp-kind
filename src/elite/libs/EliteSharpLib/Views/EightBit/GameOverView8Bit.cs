// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit game over screen: authored for the 320x256 canvas and its fixed
/// 8x8 font. The wreckage tumbling behind it is drawn by the universe, not
/// here.
/// </summary>
internal sealed class GameOverView8Bit : IView<GameOverModel>
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorGold;

    internal GameOverView8Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(GameOverModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.Graphics.DrawTextCentre(_draw.Centre.Y, model.Message, nameof(FontType.Small), _colorGold);
    }
}
