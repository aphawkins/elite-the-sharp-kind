// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit equip-ship screen: a first-draft 320x256 layout, not derived
/// from the 16-bit one - see docs/backlog-roadmap.md's "Author the 8-bit
/// view layouts" item. Exact spacing is expected to be refined visually.
/// </summary>
internal sealed class EquipmentView8Bit : BaseView8Bit, IView<EquipmentModel>
{
    private const int NameX = 8;
    private const int IndentedNameX = 24;
    private const int PriceRightX = 280;
    private const int FirstRowY = 32;
    private const int RowSpacingY = 8;
    private const int CashY = 190;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorLightGray;
    private readonly FastColor _colorRed;
    private readonly FastColor _colorWhite;

    internal EquipmentView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorLightGray = draw.Palette["LightGray"];
        _colorRed = draw.Palette["Red"];
    }

    public void Draw(EquipmentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawViewHeader("EQUIP SHIP");

        float y = FirstRowY;

        foreach (EquipmentRow row in model.Rows)
        {
            if (row.IsHighlighted)
            {
                _draw.Graphics.DrawRectangleFilled(new(2 + _draw.Layout.Offset, y), 316, RowSpacingY, _colorRed);
            }

            FastColor color = row.IsAffordable ? _colorWhite : _colorLightGray;
            int x = row.IsIndented ? IndentedNameX : NameX;
            _draw.Graphics.DrawTextLeft(new(x + _draw.Layout.Offset, y), row.Name, nameof(FontType.Small), color);

            if (row.Price.Length > 0)
            {
                _draw.Graphics.DrawTextRight(new(PriceRightX + _draw.Layout.Offset, y), row.Price, nameof(FontType.Small), color);
            }

            y += RowSpacingY;
        }

        _draw.Graphics.DrawTextLeft(new(NameX + _draw.Layout.Offset, CashY), model.Cash, nameof(FontType.Small), _colorWhite);
    }
}
