// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Trader;

namespace Elite.Engine.Views
{
    internal sealed class InventoryView : IView
    {
        private readonly Draw _draw;
        private readonly IGraphics _graphics;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;

        internal InventoryView(IGraphics graphics, Draw draw, PlayerShip ship, Trade trade)
        {
            _graphics = graphics;
            _draw = draw;
            _ship = ship;
            _trade = trade;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("INVENTORY");

            _graphics.DrawTextLeft(16, 50, "Fuel:", Colour.Green1);
            _graphics.DrawTextLeft(70, 50, $"{_ship.Fuel:N1} Light Years", Colour.White1);

            _graphics.DrawTextLeft(16, 66, "Cash:", Colour.Green1);
            _graphics.DrawTextLeft(70, 66, $"{_trade._credits:N1} Credits", Colour.White1);

            int y = 98;
            foreach (KeyValuePair<StockType, StockItem> stock in _trade._stockMarket)
            {
                if (stock.Value.CurrentCargo > 0)
                {
                    _graphics.DrawTextLeft(16, y, stock.Value.Name, Colour.White1);
                    _graphics.DrawTextLeft(180, y, $"{stock.Value.CurrentCargo}{stock.Value.Units}", Colour.White1);
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
