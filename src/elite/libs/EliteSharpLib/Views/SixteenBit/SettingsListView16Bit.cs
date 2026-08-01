// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit settings list: the 512-space layout, and nothing else. Shared
/// by the game and engine settings screens, since neither varies the layout.
/// One setting per row, name and value side by side, the rows running
/// consecutively down a single column centred on the viewport, with the Back
/// row near the foot of it rather than immediately under the list.
/// </summary>
internal sealed class SettingsListView16Bit : BaseView16Bit, IView<SettingsListModel>
{
    // A row is the font's line height, so consecutive rows leave no gap.
    private const int RowHeight = 16;
    private const int FirstRowOffset = 60;

    // The list is centred on the viewport: ListWidth is the block the name and
    // its value share, and the value sits ValueOffsetX into it.
    private const int ListWidth = 260;
    private const int ValueOffsetX = 120;
    private const int HighlightHeight = 15;

    // The Back row's distance up from the bottom of the viewport, and the
    // footer's below it.
    private const int BackRowOffset = 80;
    private const int FooterGap = 40;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorLightRed;

    internal SettingsListView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorLightRed = draw.Palette["LightRed"];
    }

    public void Draw(SettingsListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Header);

        int lastIndex = model.Rows.Count - 1;
        float listLeft = _draw.Layout.ViewportCentre.X - (ListWidth / 2);

        for (int i = 0; i < lastIndex; i++)
        {
            Vector2 position = new(
                listLeft,
                _draw.Layout.ViewportTop + FirstRowOffset + (i * RowHeight));

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(position, ListWidth, HighlightHeight, _colorLightRed);
            }

            _draw.Graphics.DrawTextLeft(position, model.Rows[i].Name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextLeft(
                new(listLeft + ValueOffsetX, position.Y),
                model.Rows[i].Value,
                nameof(FontType.Small),
                _colorWhite);
        }

        DrawBackRow(model, lastIndex);
    }

    // The Back row is centred near the foot of the viewport, with the footer
    // under it.
    private void DrawBackRow(SettingsListModel model, int lastIndex)
    {
        float y = _draw.Layout.ViewportBottom - BackRowOffset;

        if (lastIndex == model.HighlightedIndex)
        {
            _draw.Graphics.DrawRectangleFilled(
                new(_draw.Layout.ViewportCentre.X - (ListWidth / 2), y),
                ListWidth,
                HighlightHeight,
                _colorLightRed);
        }

        _draw.Graphics.DrawTextCentre(y, model.Rows[lastIndex].Name, nameof(FontType.Small), _colorWhite);

        if (model.Footer.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(y + FooterGap, model.Footer, nameof(FontType.Small), _colorWhite);
        }
    }
}
