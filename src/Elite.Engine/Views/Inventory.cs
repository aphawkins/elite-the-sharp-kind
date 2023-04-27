/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine.Views
{
    using System.Diagnostics;
    using Elite.Engine.Enums;
    using Elite.Engine.Ships;
    using Elite.Engine.Types;

    internal class Inventory : IView
    {
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;

        internal Inventory(IGfx gfx, Draw draw, PlayerShip ship, Trade trade)
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
            _gfx.DrawTextLeft(70, 50, $"{_ship.fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 66, $"{_trade.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            int y = 98;
            foreach (var stock in _trade.stock_market)
            {
                if (stock.Value.currentCargo > 0)
                {
                    _gfx.DrawTextLeft(16, y, stock.Value.name, GFX_COL.GFX_COL_WHITE);
                    _gfx.DrawTextLeft(180, y, $"{stock.Value.currentCargo}{stock.Value.units}", GFX_COL.GFX_COL_WHITE);
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