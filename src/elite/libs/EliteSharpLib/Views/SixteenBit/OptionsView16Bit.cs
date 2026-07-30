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
    private readonly FastColor _colorDarkRed;
    private readonly FastColor _colorGray;

    internal OptionsView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorDarkRed = draw.Palette["DarkRed"];
        _colorGray = draw.Palette["Gray"];
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawViewHeader("GAME OPTIONS");

        for (int i = 0; i < model.Options.Count; i++)
        {
            Vector2 position = new(
                _draw.Layout.Centre.X - (OptionBarWidth / 2),
                ((_draw.Layout.ScannerTop - (30 * model.Options.Count)) / 2) + (i * 30));

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, _colorDarkRed);
            }

            FastColor col = model.Options[i].IsEnabled ? _colorWhite : _colorGray;

            _draw.Graphics.DrawTextCentre(position.Y, model.Options[i].Label, nameof(FontType.Small), col);
        }

        _draw.Graphics.DrawTextCentre(_draw.Layout.ScannerTop - 80, model.Version, nameof(FontType.Small), _colorWhite);

        float y = _draw.Layout.ScannerTop - 60;
        foreach (string credit in model.Credits)
        {
            _draw.Graphics.DrawTextCentre(y, credit, nameof(FontType.Small), _colorWhite);
            y += 20;
        }
    }
}
