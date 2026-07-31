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
    private const int NameColumn = 1;
    private const int IndentedNameColumn = 3;
    private const int PriceRightColumn = 35;
    private const int FirstRow = 4;
    private const int CashRow = 24;

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

        DrawBorder();

        DrawViewHeader("EQUIP SHIP");

        int row = FirstRow;

        foreach (EquipmentRow equipmentRow in model.Rows)
        {
            if (equipmentRow.IsHighlighted)
            {
                _draw.Graphics.DrawRectangleFilled(new(Layout.ViewportLeft + 2, Row(row)), 316, 8, _colorRed);
            }

            FastColor color = equipmentRow.IsAffordable ? _colorWhite : _colorLightGray;
            int column = equipmentRow.IsIndented ? IndentedNameColumn : NameColumn;
            _draw.Graphics.DrawTextLeft(new(Column(column), Row(row)), equipmentRow.Name, nameof(FontType.Small), color);

            if (equipmentRow.Price.Length > 0)
            {
                _draw.Graphics.DrawTextRight(new(Column(PriceRightColumn), Row(row)), equipmentRow.Price, nameof(FontType.Small), color);
            }

            row++;
        }

        _draw.Graphics.DrawTextLeft(new(Column(NameColumn), Row(CashRow)), model.Cash, nameof(FontType.Small), _colorWhite);
    }
}
