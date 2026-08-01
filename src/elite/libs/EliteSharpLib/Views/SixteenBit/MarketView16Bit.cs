// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit market screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class MarketView16Bit : BaseView16Bit, IView<MarketModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorLightRed;

    internal MarketView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorGreen = draw.Palette["Green"];
        _colorLightRed = draw.Palette["LightRed"];
    }

    public void Draw(MarketModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Title);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 40), "PRODUCT", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(166 + _draw.Layout.ViewportLeft, 40), "UNIT", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(246 + _draw.Layout.ViewportLeft, 40), "PRICE", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(314 + _draw.Layout.ViewportLeft, 40), "FOR SALE", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(420 + _draw.Layout.ViewportLeft, 40), "IN HOLD", nameof(FontType.Small), _colorGreen);

        for (int i = 0; i < model.Rows.Count; i++)
        {
            MarketRow row = model.Rows[i];
            int y = (i * 15) + 55;

            if (row.IsHighlighted)
            {
                _draw.Graphics.DrawRectangleFilled(new(2 + _draw.Layout.ViewportLeft, y), 508, 15, _colorLightRed);
            }

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, y), row.Name, nameof(FontType.Small), _colorWhite);

            _draw.Graphics.DrawTextLeft(new(180 + _draw.Layout.ViewportLeft, y), row.Units, nameof(FontType.Small), _colorWhite);

            _draw.Graphics
                .DrawTextRight(new(285 + _draw.Layout.ViewportLeft, y), $"{row.Price:N1}", nameof(FontType.Small), _colorWhite);

            _draw.Graphics.DrawTextRight(
                new(365 + _draw.Layout.ViewportLeft, y),
                row.ForSaleQuantity > 0 ? $"{row.ForSaleQuantity}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _draw.Graphics.DrawTextLeft(
                new(365 + _draw.Layout.ViewportLeft, y),
                row.ForSaleQuantity > 0 ? row.Units : string.Empty,
                nameof(FontType.Small),
                _colorWhite);

            _draw.Graphics.DrawTextRight(
                new(455 + _draw.Layout.ViewportLeft, y),
                row.InHoldQuantity > 0 ? $"{row.InHoldQuantity,2}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _draw.Graphics.DrawTextLeft(
                new(455 + _draw.Layout.ViewportLeft, y),
                row.InHoldQuantity > 0 ? row.Units : string.Empty,
                nameof(FontType.Small),
                _colorWhite);
        }

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 340), "Cash:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics
            .DrawTextRight(new(225 + _draw.Layout.ViewportLeft, 340), $"{model.Cash,10:N1} Credits", nameof(FontType.Small), _colorWhite);
    }
}
