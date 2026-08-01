// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit settings list: authored for the 320x256 canvas and its fixed 8x8
/// font. Shared by the game and engine settings screens, exactly as the
/// 16-bit view is.
/// <para>
/// One setting per row, name and value side by side, with no blank row
/// between them: the widest row ("Graphic Style:" + "Wireframe") is 25 of the
/// 40 columns, so both fit on one line. The shared
/// <see cref="SettingsListController"/> steps its cursor one row at a time to
/// match, and the Back row sits near the foot of the viewport rather than
/// immediately under the list.
/// </para>
/// </summary>
internal sealed class SettingsListView8Bit : BaseView8Bit, IView<SettingsListModel>
{
    // Each setting is one row - name at the margin, value beside it - and the
    // rows run consecutively, in a single column.
    private const int FirstRow = 6;
    private const int CellRows = 1;
    private const int CellHeightRows = 1;
    private const int BackRow = 19;
    private const int FooterGapRows = 2;
    private const int MarginColumn = 7;
    private const int CellWidthColumns = 25;
    private const int ValueColumn = 23;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorRed;

    internal SettingsListView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorRed = draw.Palette["Red"];
    }

    public void Draw(SettingsListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Header);

        int lastIndex = model.Rows.Count - 1;

        for (int i = 0; i < lastIndex; i++)
        {
            Vector2 position = new(Column(MarginColumn), Row(FirstRow + (i * CellRows)));

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(
                    position,
                    Column(CellWidthColumns),
                    Row(CellHeightRows),
                    _colorRed);
            }

            _draw.Graphics.DrawTextLeft(position, model.Rows[i].Name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextLeft(
                new(Column(ValueColumn), position.Y),
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
        if (lastIndex == model.HighlightedIndex)
        {
            _draw.Graphics.DrawRectangleFilled(
                new(_draw.Layout.ViewportCentre.X - (Column(CellWidthColumns) / 2), Row(BackRow)),
                Column(CellWidthColumns),
                Row(CellHeightRows),
                _colorRed);
        }

        DrawTextCentreOnGrid(BackRow, model.Rows[lastIndex].Name, nameof(FontType.Small), _colorWhite);

        if (model.Footer.Length > 0)
        {
            DrawTextCentreOnGrid(BackRow + FooterGapRows, model.Footer, nameof(FontType.Small), _colorWhite);
        }
    }
}
