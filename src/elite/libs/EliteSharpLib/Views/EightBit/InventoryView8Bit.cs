// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit inventory screen: a first-draft 320x256 layout, not derived
/// from the 16-bit one - see docs/backlog-roadmap.md's "Author the 8-bit
/// view layouts" item. Exact spacing is expected to be refined visually.
/// </summary>
internal sealed class InventoryView8Bit : BaseView8Bit, IView<InventoryModel>
{
    private const int LabelColumn = 1;
    private const int ValueColumn = 7;
    private const int QuantityColumn = 17;
    private const int FirstRow = 4;
    private const int CargoFirstRow = 7;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorWhite;

    internal InventoryView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(InventoryModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Title);

        DrawRow(FirstRow, "Fuel:", model.Fuel);
        DrawRow(FirstRow + 1, "Cash:", model.Cash);

        int row = CargoFirstRow;
        foreach ((string name, string quantity) in model.Cargo)
        {
            _draw.Graphics.DrawTextLeft(new(Column(LabelColumn), Row(row)), name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextLeft(new(Column(QuantityColumn), Row(row)), quantity, nameof(FontType.Small), _colorWhite);
            row++;
        }
    }

    private void DrawRow(int row, string label, string value)
    {
        _draw.Graphics.DrawTextLeft(new(Column(LabelColumn), Row(row)), label, nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(Column(ValueColumn), Row(row)), value, nameof(FontType.Small), _colorWhite);
    }
}
