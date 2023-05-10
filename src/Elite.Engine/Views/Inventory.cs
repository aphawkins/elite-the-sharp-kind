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
        private readonly IGfx _gfx;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;

        internal InventoryView(IGfx gfx, Draw draw, PlayerShip ship, Trade trade)
        {
            _gfx = gfx;
            _draw = draw;
            _ship = ship;
            _trade = trade;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("INVENTORY");

            _gfx.DrawTextLeft(16, 50, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 50, $"{_ship.Fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 66, $"{_trade._credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            int y = 98;
            foreach (KeyValuePair<StockType, StockItem> stock in _trade._stockMarket)
            {
                if (stock.Value.CurrentCargo > 0)
                {
                    _gfx.DrawTextLeft(16, y, stock.Value.Name, GFX_COL.GFX_COL_WHITE);
                    _gfx.DrawTextLeft(180, y, $"{stock.Value.CurrentCargo}{stock.Value.Units}", GFX_COL.GFX_COL_WHITE);
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
