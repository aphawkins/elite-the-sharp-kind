// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views;

/// <summary>
/// The 16-bit inventory screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class InventoryView : IView<InventoryModel>
{
    private const int LabelX = 16;
    private const int ValueX = 70;
    private const int QuantityX = 180;
    private const int CargoStartY = 98;
    private const int SpacingY = 16;

    private readonly IEliteDraw _draw;
    private readonly uint _colorGreen;
    private readonly uint _colorWhite;

    internal InventoryView(IEliteDraw draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(InventoryModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.DrawViewHeader(model.Title);

        DrawRow(50, "Fuel:", model.Fuel);
        DrawRow(66, "Cash:", model.Cash);

        float y = CargoStartY;
        foreach ((string name, string quantity) in model.Cargo)
        {
            _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Offset, y), name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextLeft(new(QuantityX + _draw.Offset, y), quantity, nameof(FontType.Small), _colorWhite);
            y += SpacingY;
        }
    }

    private void DrawRow(float y, string label, string value)
    {
        _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Offset, y), label, nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(ValueX + _draw.Offset, y), value, nameof(FontType.Small), _colorWhite);
    }
}
