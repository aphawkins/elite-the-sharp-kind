// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Views.SixteenBit;

/// <summary>
/// The 16-bit options menu: the 512-space layout, and nothing else.
/// </summary>
internal sealed class OptionsView16Bit : BaseView16Bit, IView<OptionsModel>
{
    private const int OptionBarHeight = 15;
    private const int OptionBarWidth = 400;

    private readonly IViewSurface _surface;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorLightRed;
    private readonly FastColor _colorLightGrey;

    internal OptionsView16Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _colorWhite = surface.Palette["White"];
        _colorLightRed = surface.Palette["LightRed"];
        _colorLightGrey = surface.Palette["LightGrey"];
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("GAME OPTIONS");

        for (int i = 0; i < model.Options.Count; i++)
        {
            Vector2 position = new(
                _surface.Layout.ViewportCentre.X - (OptionBarWidth / 2),
                ((_surface.Layout.ViewportHeight - (30 * model.Options.Count)) / 2) + (i * 30));

            if (i == model.HighlightedIndex)
            {
                _surface.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, _colorLightRed);
            }

            FastColor col = model.Options[i].IsEnabled ? _colorWhite : _colorLightGrey;

            _surface.Graphics.DrawTextCentre(position.Y, model.Options[i].Label, nameof(FontType.Small), col);
        }

        _surface.Graphics.DrawTextCentre(_surface.Layout.ViewportHeight - 80, model.Version, nameof(FontType.Small), _colorWhite);

        float y = _surface.Layout.ViewportHeight - 60;
        foreach (string credit in model.Credits)
        {
            _surface.Graphics.DrawTextCentre(y, credit, nameof(FontType.Small), _colorWhite);
            y += 20;
        }
    }
}
