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
    private const int FirstRowY = 48;
    private const int CellSpacingY = 20;
    private const int CellWidth = 148;
    private const int CellHeight = 17;
    private const int ColumnPitch = 156;
    private const int MarginX = 8;
    private const int ValueIndentX = 8;
    private const int ValueOffsetY = 8;
    private const int BackRowGapY = 8;
    private const int FooterGapY = 16;

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
                MarginX + ((i & 1) * ColumnPitch) + _draw.Layout.ScannerLeft,
                FirstRowY + (i / 2 * CellSpacingY));

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(position, CellWidth, CellHeight, _colorRed);
            }

            _draw.Graphics.DrawTextLeft(position, model.Rows[i].Name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextLeft(
                new(position.X + ValueIndentX, position.Y + ValueOffsetY),
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
        float y = FirstRowY + ((lastIndex + 1) / 2 * CellSpacingY) + BackRowGapY;

        if (lastIndex == model.HighlightedIndex)
        {
            _draw.Graphics.DrawRectangleFilled(
                new(_draw.Layout.ViewportCentre.X - (CellWidth / 2), y),
                CellWidth,
                CellHeight - ValueOffsetY,
                _colorRed);
        }

        _draw.Graphics.DrawTextCentre(y, model.Rows[lastIndex].Name, nameof(FontType.Small), _colorWhite);

        if (model.Footer.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(y + FooterGapY, model.Footer, nameof(FontType.Small), _colorWhite);
        }
    }
}
