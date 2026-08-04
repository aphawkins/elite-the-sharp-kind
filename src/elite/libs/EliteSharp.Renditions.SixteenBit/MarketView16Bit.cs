// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The 16-bit market screen, laid out against the 640-wide viewport: the
/// left margin (16) is unchanged from the 512-wide layout, and every other
/// column is stretched by the same 640/512 ratio the columns themselves
/// were spread across.
/// </summary>
internal sealed class MarketView16Bit : BaseView16Bit, IView<MarketModel>
{
    private readonly IViewSurface _surface;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorLightRed;

    internal MarketView16Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _colorWhite = surface.Palette["White"];
        _colorGreen = surface.Palette["Green"];
        _colorLightRed = surface.Palette["LightRed"];
    }

    public void Draw(MarketModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Title);

        _surface.Graphics.DrawTextLeft(new(16, 40), "PRODUCT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(204, 40), "UNIT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(304, 40), "PRICE", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(389, 40), "FOR SALE", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(521, 40), "IN HOLD", nameof(FontType.Small), _colorGreen);

        for (int i = 0; i < model.Rows.Count; i++)
        {
            MarketRow row = model.Rows[i];
            int y = (i * 15) + 55;

            if (row.IsHighlighted)
            {
                _surface.Graphics.DrawRectangleFilled(new(2, y), (int)_surface.Layout.ViewportWidth - 4, 15, _colorLightRed);
            }

            _surface.Graphics.DrawTextLeft(new(16, y), row.Name, nameof(FontType.Small), _colorWhite);

            _surface.Graphics.DrawTextLeft(new(221, y), row.Units, nameof(FontType.Small), _colorWhite);

            _surface.Graphics
                .DrawTextRight(new(352, y), $"{row.Price:N1}", nameof(FontType.Small), _colorWhite);

            _surface.Graphics.DrawTextRight(
                new(452, y),
                row.ForSaleQuantity > 0 ? $"{row.ForSaleQuantity}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(452, y),
                row.ForSaleQuantity > 0 ? row.Units : string.Empty,
                nameof(FontType.Small),
                _colorWhite);

            _surface.Graphics.DrawTextRight(
                new(566, y),
                row.InHoldQuantity > 0 ? $"{row.InHoldQuantity,2}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(566, y),
                row.InHoldQuantity > 0 ? row.Units : string.Empty,
                nameof(FontType.Small),
                _colorWhite);
        }

        _surface.Graphics.DrawTextLeft(new(16, 340), "Cash:", nameof(FontType.Small), _colorGreen);
        _surface.Graphics
            .DrawTextRight(new(277, 340), $"{model.Cash,10:N1} Credits", nameof(FontType.Small), _colorWhite);
    }
}
