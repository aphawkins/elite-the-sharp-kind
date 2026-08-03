// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

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

    private readonly IViewSurface _surface;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorLightRed;

    internal SettingsListView16Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _colorWhite = surface.Palette["White"];
        _colorLightRed = surface.Palette["LightRed"];
    }

    public void Draw(SettingsListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Header);

        int lastIndex = model.Rows.Count - 1;
        float listLeft = _surface.Layout.ViewportCentre.X - (ListWidth / 2);

        for (int i = 0; i < lastIndex; i++)
        {
            Vector2 position = new(
                listLeft,
                _surface.Layout.ViewportTop + FirstRowOffset + (i * RowHeight));

            if (i == model.HighlightedIndex)
            {
                _surface.Graphics.DrawRectangleFilled(position, ListWidth, HighlightHeight, _colorLightRed);
            }

            _surface.Graphics.DrawTextLeft(position, model.Rows[i].Name, nameof(FontType.Small), _colorWhite);
            _surface.Graphics.DrawTextLeft(
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
        float y = _surface.Layout.ViewportBottom - BackRowOffset;

        if (lastIndex == model.HighlightedIndex)
        {
            _surface.Graphics.DrawRectangleFilled(
                new(_surface.Layout.ViewportCentre.X - (ListWidth / 2), y),
                ListWidth,
                HighlightHeight,
                _colorLightRed);
        }

        _surface.Graphics.DrawTextCentre(y, model.Rows[lastIndex].Name, nameof(FontType.Small), _colorWhite);

        if (model.Footer.Length > 0)
        {
            _surface.Graphics.DrawTextCentre(y + FooterGap, model.Footer, nameof(FontType.Small), _colorWhite);
        }
    }
}
