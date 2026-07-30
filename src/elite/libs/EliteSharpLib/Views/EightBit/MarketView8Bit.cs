// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit market screen: a first-draft 320x256 layout, not derived from
/// the 16-bit one - see docs/backlog-roadmap.md's "Author the 8-bit view
/// layouts" item. Drops the 16-bit view's separate "Unit" column (folded
/// into the quantity text instead, e.g. "20t") since five columns does not
/// fit a 320px/8px-per-character screen; exact spacing is expected to be
/// refined visually.
/// </summary>
internal sealed class MarketView8Bit : IView<MarketModel>
{
    private const int NameX = 8;
    private const int PriceRightX = 152;
    private const int ForSaleRightX = 216;
    private const int InHoldRightX = 280;
    private const int HeaderY = 32;
    private const int FirstRowY = 42;
    private const int RowSpacingY = 8;
    private const int CashY = 186;

    private readonly IEliteDraw _draw;
    private readonly uint _colorWhite;
    private readonly uint _colorGreen;
    private readonly uint _colorLightRed;

    internal MarketView8Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorGreen = draw.Palette["Green"];
        _colorLightRed = draw.Palette["LightRed"];
    }

    public void Draw(MarketModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.DrawViewHeader(model.Title);

        _draw.Graphics.DrawTextLeft(new(NameX + _draw.Offset, HeaderY), "ITEM", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextRight(new(PriceRightX + _draw.Offset, HeaderY), "PRICE", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextRight(new(ForSaleRightX + _draw.Offset, HeaderY), "SALE", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextRight(new(InHoldRightX + _draw.Offset, HeaderY), "HOLD", nameof(FontType.Small), _colorGreen);

        for (int i = 0; i < model.Rows.Count; i++)
        {
            MarketRow row = model.Rows[i];
            float y = FirstRowY + (i * RowSpacingY);

            if (row.IsHighlighted)
            {
                _draw.Graphics.DrawRectangleFilled(new(2 + _draw.Offset, y), 316, RowSpacingY, _colorLightRed);
            }

            _draw.Graphics.DrawTextLeft(new(NameX + _draw.Offset, y), row.Name, nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextRight(new(PriceRightX + _draw.Offset, y), $"{row.Price:N1}", nameof(FontType.Small), _colorWhite);
            _draw.Graphics.DrawTextRight(
                new(ForSaleRightX + _draw.Offset, y),
                row.ForSaleQuantity > 0 ? $"{row.ForSaleQuantity}{row.Units}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _draw.Graphics.DrawTextRight(
                new(InHoldRightX + _draw.Offset, y),
                row.InHoldQuantity > 0 ? $"{row.InHoldQuantity}{row.Units}" : "-",
                nameof(FontType.Small),
                _colorWhite);
        }

        _draw.Graphics.DrawTextLeft(new(NameX + _draw.Offset, CashY), "Cash:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextRight(
            new(InHoldRightX + _draw.Offset, CashY), $"{model.Cash:N1} Credits", nameof(FontType.Small), _colorWhite);
    }
}
