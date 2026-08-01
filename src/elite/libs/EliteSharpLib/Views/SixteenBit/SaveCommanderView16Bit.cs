// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit save-commander screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class SaveCommanderView16Bit : BaseView16Bit, IView<SaveCommanderModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorGold;

    internal SaveCommanderView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(SaveCommanderModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("SAVE COMMANDER");

        _draw.Graphics.DrawTextCentre(75, "Please enter commander name:", nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawRectangle(new(100 + _draw.Layout.ViewportLeft, 100), 312, 50, _colorWhite);
        _draw.Graphics.DrawTextCentre(112, model.Name, nameof(FontType.Large), _colorWhite);

        if (model.StatusMessage.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(175, model.StatusMessage, nameof(FontType.Large), _colorGold);
            _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", nameof(FontType.Small), _colorWhite);
        }
    }
}
