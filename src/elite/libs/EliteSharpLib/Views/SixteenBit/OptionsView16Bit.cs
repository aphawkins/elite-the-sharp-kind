// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit options menu: the 512-space layout, and nothing else.
/// </summary>
internal sealed class OptionsView16Bit : BaseView16Bit, IView<OptionsModel>
{
    private const int OptionBarHeight = 15;
    private const int OptionBarWidth = 400;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorLightRed;
    private readonly FastColor _colorLightGrey;

    internal OptionsView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorLightRed = draw.Palette["LightRed"];
        _colorLightGrey = draw.Palette["LightGrey"];
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("GAME OPTIONS");

        for (int i = 0; i < model.Options.Count; i++)
        {
            Vector2 position = new(
                _draw.Layout.ViewportCentre.X - (OptionBarWidth / 2),
                ((_draw.Layout.ViewportHeight - (30 * model.Options.Count)) / 2) + (i * 30));

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, _colorLightRed);
            }

            FastColor col = model.Options[i].IsEnabled ? _colorWhite : _colorLightGrey;

            _draw.Graphics.DrawTextCentre(position.Y, model.Options[i].Label, nameof(FontType.Small), col);
        }

        _draw.Graphics.DrawTextCentre(_draw.Layout.ViewportHeight - 80, model.Version, nameof(FontType.Small), _colorWhite);

        float y = _draw.Layout.ViewportHeight - 60;
        foreach (string credit in model.Credits)
        {
            _draw.Graphics.DrawTextCentre(y, credit, nameof(FontType.Small), _colorWhite);
            y += 20;
        }
    }
}
