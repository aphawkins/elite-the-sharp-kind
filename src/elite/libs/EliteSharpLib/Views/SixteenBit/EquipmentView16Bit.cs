// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit equip-ship screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class EquipmentView16Bit : BaseView16Bit, IView<EquipmentModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorLightGrey;
    private readonly FastColor _colorLightRed;
    private readonly FastColor _colorWhite;

    internal EquipmentView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorLightGrey = draw.Palette["LightGrey"];
        _colorLightRed = draw.Palette["LightRed"];
    }

    public void Draw(EquipmentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader("EQUIP SHIP");

        float y = 55;

        foreach (EquipmentRow row in model.Rows)
        {
            if (row.IsHighlighted)
            {
                _draw.Graphics.DrawRectangleFilled(new(2 + _draw.Layout.ViewportLeft, y + 1), 508, 15, _colorLightRed);
            }

            FastColor color = row.IsAffordable ? _colorWhite : _colorLightGrey;
            int x = row.IsIndented ? 50 : 16;
            _draw.Graphics.DrawTextLeft(new(x + _draw.Layout.ViewportLeft, y), row.Name, nameof(FontType.Small), color);

            if (row.Price.Length > 0)
            {
                _draw.Graphics.DrawTextRight(new(450 + _draw.Layout.ViewportLeft, y), row.Price, nameof(FontType.Small), color);
            }

            y += 15;
        }

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 340), model.Cash, nameof(FontType.Small), _colorWhite);
    }
}
