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
/// The two columns are kept, even though the widest row
/// ("Graphic Style:" + "Wireframe", 23 characters) leaves no room for a
/// value column beside the name at this width: the shared
/// <see cref="SettingsListController"/>'s cursor moves in steps of two, so a
/// single visual column would make Up/Down appear to skip a row. Instead each
/// cell stacks its value under its name, which keeps the grid two-wide and
/// the navigation honest.
/// </para>
/// </summary>
internal sealed class SettingsListView8Bit : BaseView8Bit, IView<SettingsListModel>
{
    // Each setting is a two-row cell - name above value - with a blank row
    // between cells, laid out in two columns.
    private const int FirstRow = 6;
    private const int CellRows = 3;
    private const int CellHeightRows = 2;
    private const int ValueOffsetRows = 1;
    private const int BackRowGapRows = 1;
    private const int FooterGapRows = 2;
    private const int MarginColumn = 1;
    private const int ColumnPitchColumns = 20;
    private const int CellWidthColumns = 18;
    private const int ValueIndentColumns = 1;

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
            Vector2 position = new(
                Column(MarginColumn + ((i & 1) * ColumnPitchColumns)),
                Row(FirstRow + (i / 2 * CellRows)));

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
                new(position.X + Column(ValueIndentColumns), position.Y + Row(ValueOffsetRows)),
                model.Rows[i].Value,
                nameof(FontType.Small),
                _colorWhite);
        }

        DrawBackRow(model, lastIndex);
    }

    // The Back row spans both columns, under the grid.
    private void DrawBackRow(SettingsListModel model, int lastIndex)
    {
        // lastIndex settings fill ceil(lastIndex / 2) grid rows above it.
        int row = FirstRow + ((lastIndex + 1) / 2 * CellRows) + BackRowGapRows;

        if (lastIndex == model.HighlightedIndex)
        {
            _draw.Graphics.DrawRectangleFilled(
                new(_draw.Layout.ViewportCentre.X - (Column(CellWidthColumns) / 2), Row(row)),
                Column(CellWidthColumns),
                Row(CellHeightRows - ValueOffsetRows),
                _colorRed);
        }

        DrawTextCentreOnGrid(row, model.Rows[lastIndex].Name, nameof(FontType.Small), _colorWhite);

        if (model.Footer.Length > 0)
        {
            DrawTextCentreOnGrid(row + FooterGapRows, model.Footer, nameof(FontType.Small), _colorWhite);
        }
    }
}
