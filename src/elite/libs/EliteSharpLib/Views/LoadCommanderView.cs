// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views;

/// <summary>
/// The 16-bit load-commander screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class LoadCommanderView : IView<LoadCommanderModel>
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorWhite;
    private readonly uint _colorGold;

    internal LoadCommanderView(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(LoadCommanderModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.DrawViewHeader("LOAD COMMANDER");

        _draw.Graphics.DrawTextCentre(75, "Please enter commander name:", nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawRectangleCentre(100, 312, 50, _colorWhite);
        _draw.Graphics.DrawTextCentre(112, model.Name, nameof(FontType.Large), _colorWhite);

        if (model.ErrorMessage.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(175, model.ErrorMessage, nameof(FontType.Large), _colorGold);
            _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", nameof(FontType.Small), _colorWhite);
        }
    }
}
