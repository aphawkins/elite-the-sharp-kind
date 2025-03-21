// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Trader;

namespace EliteSharp.Views;

internal sealed class InventoryView : IView
{
    private readonly IDraw _draw;
    private readonly PlayerShip _ship;
    private readonly Trade _trade;

    internal InventoryView(IDraw draw, PlayerShip ship, Trade trade)
    {
        _draw = draw;
        _ship = ship;
        _trade = trade;
    }

    public void Draw()
    {
        _draw.DrawViewHeader("INVENTORY");

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 50), "Fuel:", EliteColors.Green);
        _draw.Graphics.DrawTextLeft(new(70 + _draw.Offset, 50), $"{_ship.Fuel:N1} Light Years", EliteColors.White);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 66), "Cash:", EliteColors.Green);
        _draw.Graphics.DrawTextLeft(new(70 + _draw.Offset, 66), $"{_trade.Credits:N1} Credits", EliteColors.White);

        int y = 98;
        foreach (KeyValuePair<StockType, StockItem> stock in _trade.StockMarket)
        {
            if (stock.Value.CurrentCargo > 0)
            {
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, y), stock.Value.Name, EliteColors.White);
                _draw.Graphics.DrawTextLeft(
                    new(180 + _draw.Offset, y),
                    $"{stock.Value.CurrentCargo}{stock.Value.Units}",
                    EliteColors.White);
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
