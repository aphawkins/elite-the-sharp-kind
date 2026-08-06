// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful.UI;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The 16-bit options menu: the 512-space layout, and nothing else.
/// <para>
/// Two stacks - the options and the credits under them - each a container the
/// rows are centred in. The option rows are the width of the selection bar,
/// so a selected row's block is the bar; the credits are the full viewport
/// width, which is how they land where screen-centred text used to.
/// </para>
/// </summary>
internal sealed class OptionsView16Bit : BaseView16Bit, IView<OptionsModel>
{
    private const int OptionBarHeight = 15;
    private const int OptionBarWidth = 400;
    private const int OptionSpacing = 30;
    private const int VersionOffsetY = 80;
    private const int CreditsOffsetY = 60;
    private const int CreditSpacing = 20;

    private readonly IViewSurface _surface;
    private readonly ControlStyle _style;
    private readonly Container<Label> _options;
    private readonly Container<Label> _credits;
    private readonly Label _version;

    internal OptionsView16Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _style = new(
            nameof(FontType.Small),
            ControlColors.TextOnly(surface.Palette["White"]),
            new(surface.Palette["LightRed"], surface.Palette["White"]),
            ControlColors.TextOnly(surface.Palette["LightGrey"]));

        _options = new(surface.Graphics, _style) { ChildAlignment = TextAlignment.Centre, Spacing = OptionSpacing };
        _credits = new(surface.Graphics, _style) { ChildAlignment = TextAlignment.Centre, Spacing = CreditSpacing };

        _version = new(surface.Graphics, _style, new TextSetting())
        {
            Alignment = TextAlignment.Centre,
            Width = surface.Layout.ViewportWidth,
        };
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("GAME OPTIONS");

        FillOptions(model);
        FillCredits(model);

        _options.Width = OptionBarWidth;
        _options.Position = new(
            _surface.Layout.ViewportCentre.X - (OptionBarWidth / 2),
            (_surface.Layout.ViewportHeight - (OptionSpacing * model.Options.Count)) / 2);
        _options.Draw();

        _version.Position = new(_surface.Layout.ViewportLeft, _surface.Layout.ViewportHeight - VersionOffsetY);
        _version.Setting.Value = model.Version;
        _version.Draw();

        _credits.Width = _surface.Layout.ViewportWidth;
        _credits.Position = new(_surface.Layout.ViewportLeft, _surface.Layout.ViewportHeight - CreditsOffsetY);
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
                _options.Add(new Label(_surface.Graphics, _style, new TextSetting())
                {
                    Alignment = TextAlignment.Centre,
                    Width = OptionBarWidth,
                    Height = OptionBarHeight,
                });
            }
        }

        for (int i = 0; i < model.Options.Count; i++)
        {
            Label row = _options.Children[i];
            row.Setting.Value = model.Options[i].Label;
            row.State = (i == model.HighlightedIndex, model.Options[i].IsEnabled) switch
            {
                (true, true) => ControlState.Selected,
                (true, false) => ControlState.SelectedDisabled,
                (false, true) => ControlState.Normal,
                (false, false) => ControlState.Disabled,
            };
        }
    }

    private void FillCredits(OptionsModel model)
    {
        if (_credits.Children.Count != model.Credits.Count)
        {
            _credits.Clear();
            for (int i = 0; i < model.Credits.Count; i++)
            {
                _credits.Add(new Label(_surface.Graphics, _style, new TextSetting())
                {
                    Alignment = TextAlignment.Centre,
                    Width = _surface.Layout.ViewportWidth,
                });
            }
        }

        for (int i = 0; i < model.Credits.Count; i++)
        {
            _credits.Children[i].Setting.Value = model.Credits[i];
        }
    }
}
