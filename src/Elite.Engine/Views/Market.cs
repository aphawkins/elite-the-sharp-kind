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
    using Elite.Engine.Enums;

    internal class Market : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private int _highlightedStock;

        internal Market(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            _highlightedStock = 0;
        }

        public void UpdateUniverse()
        {
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader($"{Planet.name_planet(elite.docked_planet, false)} MARKET PRICES");

            _gfx.DrawTextLeft(16, 40, "PRODUCT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(166, 40, "UNIT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(246, 40, "PRICE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(314, 40, "FOR SALE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(420, 40, "IN HOLD", GFX_COL.GFX_COL_GREEN_1);

            for (int i = 0; i < trade.stock_market.Length; i++)
            {
                int y = (i * 15) + 55;

                if (i == _highlightedStock)
                {
                    _gfx.DrawRectangleFilled(2, y, 508, 15, GFX_COL.GFX_COL_DARK_RED);
                }

                _gfx.DrawTextLeft(16, y, trade.stock_market[i].name, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextLeft(180, y, trade.stock_market[i].units, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(285, y, $"{trade.stock_market[i].current_price:N1}", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(365, y, trade.stock_market[i].current_quantity > 0 ? $"{trade.stock_market[i].current_quantity}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(365, y, trade.stock_market[i].current_quantity > 0 ? trade.stock_market[i].units : "", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(455, y, elite.cmdr.current_cargo[i] > 0 ? $"{elite.cmdr.current_cargo[i],2}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(455, y, elite.cmdr.current_cargo[i] > 0 ? trade.stock_market[i].units : "", GFX_COL.GFX_COL_WHITE);
            }

            _gfx.DrawTextLeft(16, 340, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextRight(160, 340, $"{elite.cmdr.credits,10:N1} Credits", GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up))
            {
                _highlightedStock = Math.Clamp(_highlightedStock - 1, 0, trade.stock_market.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down))
            {
                _highlightedStock = Math.Clamp(_highlightedStock + 1, 0, trade.stock_market.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left))
            {
                SellStock();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right))
            {
                BuyStock();
            }
        }

        private void BuyStock()
        {
            if (!elite.docked)
            {
                return;
            }

            if (trade.stock_market[_highlightedStock].current_quantity == 0 || elite.cmdr.credits < trade.stock_market[_highlightedStock].current_price)
            {
                return;
            }

            if (trade.stock_market[_highlightedStock].units == trade.TONNES && trade.total_cargo() == elite.cmdr.cargo_capacity)
            {
                return;
            }

            elite.cmdr.current_cargo[_highlightedStock]++;
            trade.stock_market[_highlightedStock].current_quantity--;
            elite.cmdr.credits -= trade.stock_market[_highlightedStock].current_price;
        }

        private void SellStock()
        {
            if (!elite.docked)
            {
                return;
            }

            if (elite.cmdr.current_cargo[_highlightedStock] == 0)
            {
                return;
            }

            elite.cmdr.current_cargo[_highlightedStock]--;
            trade.stock_market[_highlightedStock].current_quantity++;
            elite.cmdr.credits += trade.stock_market[_highlightedStock].current_price;
        }
    }
}