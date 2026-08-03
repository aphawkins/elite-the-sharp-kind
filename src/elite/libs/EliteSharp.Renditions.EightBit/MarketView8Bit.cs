// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The 8-bit market screen: a first-draft 320x256 layout, not derived from
/// the 16-bit one - see docs/backlog-roadmap.md's "Author the 8-bit view
/// layouts" item. Drops the 16-bit view's separate "Unit" column (folded
/// into the quantity text instead, e.g. "20t") since five columns does not
/// fit a 320px/8px-per-character screen; exact spacing is expected to be
/// refined visually.
/// </summary>
internal sealed class MarketView8Bit : BaseView8Bit, IView<MarketModel>
{
    private const int NameColumn = 1;
    private const int UnitRightColumn = 16;
    private const int PriceRightColumn = 24;
    private const int ForSaleRightColumn = 29;
    private const int InHoldRightColumn = 37;
    private const int HeaderRow = 3;
    private const int FirstRow = 5;
    private const int CashRow = 23;

    private readonly IViewSurface _surface;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorRed;

    internal MarketView8Bit(IViewSurface surface)
        : base(surface)
    {
        _surface = surface;

        _colorWhite = surface.Palette["White"];
        _colorGreen = surface.Palette["Green"];
        _colorRed = surface.Palette["Red"];
    }

    public void Draw(MarketModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Title);

        _surface.Graphics.DrawTextLeft(new(Column(NameColumn), Row(HeaderRow + 1)), "PRODUCT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(new(Column(UnitRightColumn + 2), Row(HeaderRow + 1)), "UNIT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(new(Column(PriceRightColumn + 1), Row(HeaderRow)), "UNIT", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(
            new(Column(PriceRightColumn + 1), Row(HeaderRow + 1)), "PRICE", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(
            new(Column(ForSaleRightColumn + 5), Row(HeaderRow)), "QUANTITY", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(
            new(Column(ForSaleRightColumn + 5), Row(HeaderRow + 1)), "FOR SALE", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(new(Column(InHoldRightColumn + 1), Row(HeaderRow)), "IN", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(
            new(Column(InHoldRightColumn + 2), Row(HeaderRow + 1)), "HOLD", nameof(FontType.Small), _colorGreen);

        for (int i = 0; i < model.Rows.Count; i++)
        {
            MarketRow marketRow = model.Rows[i];
            int row = FirstRow + i;

            if (marketRow.IsHighlighted)
            {
                _surface.Graphics.DrawRectangleFilled(new(Layout.ViewportLeft + 2, Row(row)), 316, 8, _colorRed);
            }

            _surface.Graphics.DrawTextLeft(new(Column(NameColumn), Row(row)), marketRow.Name, nameof(FontType.Small), _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(Column(UnitRightColumn), Row(row)),
                $"{marketRow.Units}",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextRight(
                new(Column(PriceRightColumn), Row(row)),
                $"{marketRow.Price:N1}",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextRight(
                new(Column(ForSaleRightColumn), Row(row)),
                marketRow.ForSaleQuantity > 0 ? $"{marketRow.ForSaleQuantity}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(Column(ForSaleRightColumn), Row(row)),
                marketRow.ForSaleQuantity > 0 ? $"{marketRow.Units}" : string.Empty,
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextRight(
                new(Column(InHoldRightColumn), Row(row)),
                marketRow.InHoldQuantity > 0 ? $"{marketRow.InHoldQuantity}" : "-",
                nameof(FontType.Small),
                _colorWhite);
            _surface.Graphics.DrawTextLeft(
                new(Column(InHoldRightColumn), Row(row)),
                marketRow.InHoldQuantity > 0 ? $"{marketRow.Units}" : string.Empty,
                nameof(FontType.Small),
                _colorWhite);
        }

        _surface.Graphics.DrawTextLeft(new(Column(NameColumn), Row(CashRow)), "Cash:", nameof(FontType.Small), _colorGreen);
        _surface.Graphics.DrawTextRight(
            new(Column(InHoldRightColumn), Row(CashRow)), $"{model.Cash:N1} Credits", nameof(FontType.Small), _colorWhite);
    }
}
