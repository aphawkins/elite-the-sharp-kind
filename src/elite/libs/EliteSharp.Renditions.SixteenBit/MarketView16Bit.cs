// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The 16-bit market screen: the 512-space layout, and nothing else.
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

        _surface.Graphics.DrawTextLeft(new(16 + _surface.Layout.ViewportLeft, 40), "PRODUCT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(166 + _surface.Layout.ViewportLeft, 40), "UNIT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(246 + _surface.Layout.ViewportLeft, 40), "PRICE", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(314 + _surface.Layout.ViewportLeft, 40), "FOR SALE", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextLeft(new(420 + _surface.Layout.ViewportLeft, 40), "IN HOLD", nameof(FontType.Small), _colorGreen);

        for (int i = 0; i < model.Rows.Count; i++)
        {
            MarketRow row = model.Rows[i];
            int y = (i * 15) + 55;

            if (row.IsHighlighted)
            {
                _surface.Graphics.DrawRectangleFilled(new(2 + _surface.Layout.ViewportLeft, y), 508, 15, _colorLightRed);
            }

            _surface.Graphics.DrawTextLeft(new(16 + _surface.Layout.ViewportLeft, y), row.Name, nameof(FontType.Small), _colorWhite);

            _surface.Graphics.DrawTextLeft(new(180 + _surface.Layout.ViewportLeft, y), row.Units, nameof(FontType.Small), _colorWhite);

            _surface.Graphics
                .DrawTextRight(new(285 + _surface.Layout.ViewportLeft, y), $"{row.Price:N1}", nameof(FontType.Small), _colorWhite);

            _surface.Graphics.DrawTextRight(
                new(365 + _surface.Layout.ViewportLeft, y),
                row.ForSaleQuantity > 0 ? $"{row.ForSaleQuantity}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(365 + _surface.Layout.ViewportLeft, y),
                row.ForSaleQuantity > 0 ? row.Units : string.Empty,
                nameof(FontType.Small),
                _colorWhite);

            _surface.Graphics.DrawTextRight(
                new(455 + _surface.Layout.ViewportLeft, y),
                row.InHoldQuantity > 0 ? $"{row.InHoldQuantity,2}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(455 + _surface.Layout.ViewportLeft, y),
                row.InHoldQuantity > 0 ? row.Units : string.Empty,
                nameof(FontType.Small),
                _colorWhite);
        }

        _surface.Graphics.DrawTextLeft(new(16 + _surface.Layout.ViewportLeft, 340), "Cash:", nameof(FontType.Small), _colorGreen);
        _surface.Graphics
            .DrawTextRight(
                new(225 + _surface.Layout.ViewportLeft, 340), $"{model.Cash,10:N1} Credits", nameof(FontType.Small), _colorWhite);
    }
}
