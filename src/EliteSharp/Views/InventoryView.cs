// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Trader;

namespace EliteSharp.Views
{
    internal sealed class InventoryView : IView
    {
        private readonly IDraw _draw;
        private readonly IGraphics _graphics;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;

        internal InventoryView(IGraphics graphics, IDraw draw, PlayerShip ship, Trade trade)
        {
            _graphics = graphics;
            _draw = draw;
            _ship = ship;
            _trade = trade;
        }

        public void Draw()
        {
            _draw.DrawViewHeader("INVENTORY");

            _graphics.DrawTextLeft(new(16 + _draw.Offset, 50), "Fuel:", Colour.Green);
            _graphics.DrawTextLeft(new(70 + _draw.Offset, 50), $"{_ship.Fuel:N1} Light Years", Colour.White);

            _graphics.DrawTextLeft(new(16 + _draw.Offset, 66), "Cash:", Colour.Green);
            _graphics.DrawTextLeft(new(70 + _draw.Offset, 66), $"{_trade.Credits:N1} Credits", Colour.White);

            int y = 98;
            foreach (KeyValuePair<StockType, StockItem> stock in _trade.StockMarket)
            {
                if (stock.Value.CurrentCargo > 0)
                {
                    _graphics.DrawTextLeft(new(16 + _draw.Offset, y), stock.Value.Name, Colour.White);
                    _graphics.DrawTextLeft(new(180 + _draw.Offset, y), $"{stock.Value.CurrentCargo}{stock.Value.Units}", Colour.White);
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
}
