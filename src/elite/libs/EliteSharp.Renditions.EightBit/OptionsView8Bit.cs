// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The 8-bit options menu: authored for the 320x256 canvas and its fixed 8x8
/// font. The 16-bit view's 400px selection bar overflows a 320-wide screen,
/// so this tier has its own width, and the credits are word-wrapped rather
/// than drawn one centred line each: the longest ("The New Kind - Christian
/// Pinder 1999-2001", 41 characters) is one character wider than the screen's
/// 40-character row. It shares <see cref="TextWrap"/> with
/// <see cref="IBaseView.DrawTextPretty"/> but not the drawing: these rows are
/// centred rather than left-aligned, and the block is stacked upwards from the
/// bottom of the viewport, so the line count has to be known before the first
/// row is placed.
/// </summary>
internal sealed class OptionsView8Bit : BaseView8Bit, IView<OptionsModel>
{
    private const int OptionBarHeight = 8;
    private const int OptionBarWidth = 240;
    private const int FirstRow = 7;
    private const int RowSpacingRows = 2;
    private const int CreditsLastRow = 23;
    private const int VersionGapRows = 2;

    // 320px of 8x8 characters, less a character of margin either side.
    private const int MaxCharsPerLine = 38;

    private readonly IViewSurface _surface;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorRed;
    private readonly FastColor _colorLightGray;

    internal OptionsView8Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _colorWhite = surface.Palette["White"];
        _colorRed = surface.Palette["Red"];
        _colorLightGray = surface.Palette["LightGray"];
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("OPTIONS");

        for (int i = 0; i < model.Options.Count; i++)
        {
            int row = FirstRow + (i * RowSpacingRows);
            Vector2 position = new(_surface.Layout.ViewportCentre.X - (OptionBarWidth / 2), Row(row));

            if (i == model.HighlightedIndex)
            {
                _surface.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, _colorRed);
            }

            FastColor col = model.Options[i].IsEnabled ? _colorWhite : _colorLightGray;

            DrawTextCentreOnGrid(row, model.Options[i].Label, nameof(FontType.Small), col);
        }

        List<string> lines = [];
        foreach (string credit in model.Credits)
        {
            lines.AddRange(TextWrap.Split(credit, MaxCharsPerLine));
        }

        // The credits sit against the bottom of the viewport, one row each, so
        // the block grows upwards as it wraps.
        int creditsFirstRow = CreditsLastRow - lines.Count + 1;

        DrawTextCentreOnGrid(creditsFirstRow - VersionGapRows, model.Version, nameof(FontType.Small), _colorWhite);

        for (int i = 0; i < lines.Count; i++)
        {
            DrawTextCentreOnGrid(creditsFirstRow + i, lines[i], nameof(FontType.Small), _colorWhite);
        }
    }
}
