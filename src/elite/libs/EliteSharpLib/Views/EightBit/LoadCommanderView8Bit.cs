// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit load-commander screen: a first-draft 320x256 layout, not
/// derived from the 16-bit one - see docs/backlog-roadmap.md's "Author the
/// 8-bit view layouts" item. Exact spacing is expected to be refined
/// visually.
/// </summary>
internal sealed class LoadCommanderView8Bit : BaseView8Bit, IView<LoadCommanderModel>
{
    private const int BoxWidth = 280;
    private const int BoxHeight = 24;
    private const int BoxY = 64;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorYellow;

    internal LoadCommanderView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorYellow = draw.Palette["Yellow"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(LoadCommanderModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("LOAD COMMANDER");

        DrawTextCentreOnGrid(6, "Please enter commander name:", nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawRectangleCentre(BoxY, BoxWidth, BoxHeight, _colorWhite);
        DrawTextCentreOnGrid(9, model.Name, nameof(FontType.Small), _colorWhite);

        if (model.ErrorMessage.Length > 0)
        {
            DrawTextCentreOnGrid(14, model.ErrorMessage, nameof(FontType.Small), _colorYellow);
            DrawTextCentreOnGrid(16, "Press SPACE to continue.", nameof(FontType.Small), _colorWhite);
        }
    }
}
