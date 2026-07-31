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
    private const int LabelX = 8;
    private const int ValueX = 48;
    private const int QuantityX = 136;
    private const int FirstRowY = 40;
    private const int RowSpacingY = 8;
    private const int CargoStartY = 60;

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

        DrawRow(FirstRowY, "Fuel:", model.Fuel);
        DrawRow(FirstRowY + RowSpacingY, "Cash:", model.Cash);

        float y = CargoStartY;
        foreach ((string name, string quantity) in model.Cargo)
        {
            _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Layout.ScannerLeft, y), name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextLeft(new(QuantityX + _draw.Layout.ScannerLeft, y), quantity, nameof(FontType.Small), _colorWhite);
            y += RowSpacingY;
        }
    }

    private void DrawRow(float y, string label, string value)
    {
        _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Layout.ScannerLeft, y), label, nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(ValueX + _draw.Layout.ScannerLeft, y), value, nameof(FontType.Small), _colorWhite);
    }
}
