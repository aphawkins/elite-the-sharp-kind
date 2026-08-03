// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The 16-bit inventory screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class InventoryView16Bit : BaseView16Bit, IView<InventoryModel>
{
    private const int LabelX = 16;
    private const int ValueX = 70;
    private const int QuantityX = 180;
    private const int CargoStartY = 98;
    private const int SpacingY = 16;

    private readonly IViewSurface _surface;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorWhite;

    internal InventoryView16Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _colorGreen = surface.Palette["Green"];
        _colorWhite = surface.Palette["White"];
    }

    public void Draw(InventoryModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Title);

        DrawRow(50, "Fuel:", model.Fuel);
        DrawRow(66, "Cash:", model.Cash);

        float y = CargoStartY;
        foreach ((string name, string quantity) in model.Cargo)
        {
            _surface.Graphics.DrawTextLeft(new(LabelX + _surface.Layout.ViewportLeft, y), name, nameof(FontType.Small), _colorWhite);
            _surface.Graphics.DrawTextLeft(new(QuantityX + _surface.Layout.ViewportLeft, y), quantity, nameof(FontType.Small), _colorWhite);
            y += SpacingY;
        }
    }

    private void DrawRow(float y, string label, string value)
    {
        _surface.Graphics.DrawTextLeft(new(LabelX + _surface.Layout.ViewportLeft, y), label, nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(ValueX + _surface.Layout.ViewportLeft, y), value, nameof(FontType.Small), _colorWhite);
    }
}
