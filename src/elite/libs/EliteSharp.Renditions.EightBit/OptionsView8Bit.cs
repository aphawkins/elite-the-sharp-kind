// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful.Widgets;

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
/// <para>
/// Every centred row here snaps to the character cell: this tier's font is a
/// fixed 8x8, and pixel-centring an odd-length string lands it on half a cell.
/// </para>
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
    private readonly WidgetStyle _style;
    private readonly Container<Label> _options = new() { ChildAlignment = TextAlignment.Centre, Spacing = RowSpacingRows * RowHeight };
    private readonly Container<Label> _credits = new() { ChildAlignment = TextAlignment.Centre, Spacing = RowHeight };
    private readonly Label _version;

    internal OptionsView8Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _style = new(
            nameof(FontType.Small),
            WidgetColors.TextOnly(surface.Palette["White"]),
            new(surface.Palette["Red"], surface.Palette["White"]),
            WidgetColors.TextOnly(surface.Palette["LightGray"]));

        _version = new(surface.Graphics, _style)
        {
            Alignment = TextAlignment.Centre,
            Width = surface.Layout.ViewportWidth,
            SnapToCell = CharacterWidth,
        };
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("OPTIONS");

        FillOptions(model);

        _options.Width = OptionBarWidth;
        _options.Position = new(_surface.Layout.ViewportCentre.X - (OptionBarWidth / 2), Row(FirstRow));
        _options.Draw();

        List<string> lines = [];
        foreach (string credit in model.Credits)
        {
            lines.AddRange(TextWrap.Split(credit, MaxCharsPerLine));
        }

        // The credits sit against the bottom of the viewport, one row each, so
        // the block grows upwards as it wraps.
        int creditsFirstRow = CreditsLastRow - lines.Count + 1;

        _version.Position = new(_surface.Layout.ViewportLeft, Row(creditsFirstRow - VersionGapRows));
        _version.Text = model.Version;
        _version.Draw();

        FillCredits(lines);

        _credits.Width = _surface.Layout.ViewportWidth;
        _credits.Position = new(_surface.Layout.ViewportLeft, Row(creditsFirstRow));
        _credits.Draw();
    }

    // The rows are rebuilt only when the count changes: it never does in
    // practice, but the model owns the list, so the view does not assume it.
    private void FillOptions(OptionsModel model)
    {
        if (_options.Children.Count != model.Options.Count)
        {
            _options.Clear();
            for (int i = 0; i < model.Options.Count; i++)
            {
                _options.Add(new Label(_surface.Graphics, _style)
                {
                    Alignment = TextAlignment.Centre,
                    Width = OptionBarWidth,
                    Height = OptionBarHeight,
                    SnapToCell = CharacterWidth,
                });
            }
        }

        for (int i = 0; i < model.Options.Count; i++)
        {
            Label row = _options.Children[i];
            row.Text = model.Options[i].Label;
            row.State = (i == model.HighlightedIndex, model.Options[i].IsEnabled) switch
            {
                (true, true) => WidgetState.Selected,
                (true, false) => WidgetState.SelectedDisabled,
                (false, true) => WidgetState.Normal,
                (false, false) => WidgetState.Disabled,
            };
        }
    }

    private void FillCredits(List<string> lines)
    {
        if (_credits.Children.Count != lines.Count)
        {
            _credits.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                _credits.Add(new Label(_surface.Graphics, _style)
                {
                    Alignment = TextAlignment.Centre,
                    Width = _surface.Layout.ViewportWidth,
                    SnapToCell = CharacterWidth,
                });
            }
        }

        for (int i = 0; i < lines.Count; i++)
        {
            _credits.Children[i].Text = lines[i];
        }
    }
}
