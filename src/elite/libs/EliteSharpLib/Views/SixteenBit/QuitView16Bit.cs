// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit quit confirmation: the 512-space layout, and nothing else.
/// </summary>
internal sealed class QuitView16Bit : BaseView16Bit, IView<QuitModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGold;

    internal QuitView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(QuitModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Header);

        _draw.Graphics.DrawTextCentre(_draw.Layout.ViewportCentre.Y, model.Prompt, nameof(FontType.Large), _colorGold);
    }
}
