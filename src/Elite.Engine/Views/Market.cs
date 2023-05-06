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

using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal class Market : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly Trade _trade;
        private readonly Planet _planet;
        private StockType _highlightedStock;

        internal Market(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, Trade trade, Planet planet)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _trade = trade;
            _planet = planet;
        }

        public void Reset() => _highlightedStock = 0;

        public void UpdateUniverse()
        {
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader($"{_planet.NamePlanet(_gameState.docked_planet, false)} MARKET PRICES");

            _gfx.DrawTextLeft(16, 40, "PRODUCT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(166, 40, "UNIT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(246, 40, "PRICE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(314, 40, "FOR SALE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(420, 40, "IN HOLD", GFX_COL.GFX_COL_GREEN_1);

            int i = 0;
            foreach (KeyValuePair<StockType, StockItem> stock in _trade.stockMarket)
            {
                int y = (i * 15) + 55;

                if (stock.Key == _highlightedStock)
                {
                    _gfx.DrawRectangleFilled(2, y, 508, 15, GFX_COL.GFX_COL_DARK_RED);
                }

                _gfx.DrawTextLeft(16, y, stock.Value.name, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextLeft(180, y, stock.Value.units, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(285, y, $"{stock.Value.currentPrice:N1}", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(365, y, stock.Value.currentQuantity > 0 ? $"{stock.Value.currentQuantity}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(365, y, stock.Value.currentQuantity > 0 ? stock.Value.units : "", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(455, y, stock.Value.currentCargo > 0 ? $"{stock.Value.currentCargo,2}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(455, y, stock.Value.currentCargo > 0 ? stock.Value.units : "", GFX_COL.GFX_COL_WHITE);

                i++;
            }

            _gfx.DrawTextLeft(16, 340, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextRight(160, 340, $"{_trade.credits,10:N1} Credits", GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                _highlightedStock = (StockType)Math.Clamp((int)_highlightedStock - 1, 0, _trade.stockMarket.Count - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                _highlightedStock = (StockType)Math.Clamp((int)_highlightedStock + 1, 0, _trade.stockMarket.Count - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                _trade.SellStock(_highlightedStock);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
            {
                _trade.BuyStock(_highlightedStock);
            }
        }
    }
}
