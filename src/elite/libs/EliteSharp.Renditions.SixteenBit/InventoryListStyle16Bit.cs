// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using SharpKind.UI;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// How the inventory looks in the 512-space layout, and nothing else.
/// <para>
/// The cargo starts at y=98 and the viewport stops where the HUD begins, so
/// how many sixteen-pixel rows fit is worked out from the surface rather than
/// counted by hand. That number is
/// <see cref="InventoryListStyle.VisibleRows"/>, so a hold carrying more sorts
/// of thing than fit scrolls instead of running off the bottom.
/// </para>
/// </summary>
internal static class InventoryListStyle16Bit
{
    private const float LabelX = 16;
    private const float ValueX = 70;
    private const float QuantityX = 180;
    private const float FuelY = 50;
    private const float CashY = 66;
    private const float CargoStartY = 98;
    private const float SpacingY = 16;

    internal static InventoryListStyle Create(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        ControlColors white = ControlColors.TextOnly(surface.Palette["White"]);
        ControlColors green = ControlColors.TextOnly(surface.Palette["Green"]);

        float left = surface.Layout.ViewportLeft;

        // The last row that fits whole above the HUD.
        int visibleRows = (int)((surface.Layout.ViewportHeight - CargoStartY) / SpacingY);

        return new(
            RowStyle: new(nameof(FontType.Small), white, white, white),
            CaptionStyle: new(nameof(FontType.Small), green, green, green),
            Columns:
            [
                new(0, TextAlignment.Left),
                new(QuantityX - LabelX, TextAlignment.Left),
            ],
            FuelCaption: new("Fuel:", left + LabelX, FuelY, TextAlignment.Left),
            FuelValue: new(string.Empty, left + ValueX, FuelY, TextAlignment.Left),
            CashCaption: new("Cash:", left + LabelX, CashY, TextAlignment.Left),
            CashValue: new(string.Empty, left + ValueX, CashY, TextAlignment.Left),
            RowsLeft: left + LabelX,
            FirstRowY: CargoStartY,
            RowHeight: SpacingY,
            RowWidth: surface.Layout.ViewportWidth - LabelX,
            VisibleRows: visibleRows);
    }
}
