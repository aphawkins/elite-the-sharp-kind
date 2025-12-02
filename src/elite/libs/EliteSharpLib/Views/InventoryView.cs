// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Views;

internal sealed class InventoryView : IView
{
    private readonly IEliteDraw _draw;
    private readonly PlayerShip _ship;
    private readonly Trade _trade;
    private readonly uint _colorGreen;
    private readonly uint _colorWhite;

    internal InventoryView(IEliteDraw draw, PlayerShip ship, Trade trade)
    {
        _draw = draw;
        _ship = ship;
        _trade = trade;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw()
    {
        _draw.DrawViewHeader("INVENTORY");

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 50), "Fuel:", (int)FontType.Small, _colorGreen);
        _draw.Graphics.DrawTextLeft(new(70 + _draw.Offset, 50), $"{_ship.Fuel:N1} Light Years", (int)FontType.Small, _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 66), "Cash:", (int)FontType.Small, _colorGreen);
        _draw.Graphics.DrawTextLeft(new(70 + _draw.Offset, 66), $"{_trade.Credits:N1} Credits", (int)FontType.Small, _colorWhite);

        int y = 98;
        foreach (KeyValuePair<StockType, StockItem> stock in _trade.StockMarket)
        {
            if (stock.Value.CurrentCargo > 0)
            {
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, y), stock.Value.Name, (int)FontType.Small, _colorWhite);
                _draw.Graphics.DrawTextLeft(
                    new(180 + _draw.Offset, y),
                    $"{stock.Value.CurrentCargo}{stock.Value.Units}",
                    (int)FontType.Small,
                    _colorWhite);
                y += 16;
            }
        }
    }

    public void HandleInput()
    {
    }

    public void Reset()
    {
    }

    public void UpdateUniverse()
    {
    }
}
