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
    using Elite.Engine.Ships;

    internal class Market : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly trade _trade;
        private readonly PlayerShip _ship;
        private int _highlightedStock;

        internal Market(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, trade trade, PlayerShip ship)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _trade = trade;
            _ship = ship;
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
            _draw.ClearDisplay();
            _draw.DrawViewHeader($"{Planet.name_planet(_gameState.docked_planet, false)} MARKET PRICES");

            _gfx.DrawTextLeft(16, 40, "PRODUCT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(166, 40, "UNIT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(246, 40, "PRICE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(314, 40, "FOR SALE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(420, 40, "IN HOLD", GFX_COL.GFX_COL_GREEN_1);

            for (int i = 0; i < _gameState.stock_market.Length; i++)
            {
                int y = (i * 15) + 55;

                if (i == _highlightedStock)
                {
                    _gfx.DrawRectangleFilled(2, y, 508, 15, GFX_COL.GFX_COL_DARK_RED);
                }

                _gfx.DrawTextLeft(16, y, _gameState.stock_market[i].name, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextLeft(180, y, _gameState.stock_market[i].units, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(285, y, $"{_gameState.stock_market[i].current_price:N1}", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(365, y, _gameState.stock_market[i].current_quantity > 0 ? $"{_gameState.stock_market[i].current_quantity}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(365, y, _gameState.stock_market[i].current_quantity > 0 ? _gameState.stock_market[i].units : "", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(455, y, _gameState.cmdr.current_cargo[i] > 0 ? $"{_gameState.cmdr.current_cargo[i],2}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(455, y, _gameState.cmdr.current_cargo[i] > 0 ? _gameState.stock_market[i].units : "", GFX_COL.GFX_COL_WHITE);
            }

            _gfx.DrawTextLeft(16, 340, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextRight(160, 340, $"{_gameState.cmdr.credits,10:N1} Credits", GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                _highlightedStock = Math.Clamp(_highlightedStock - 1, 0, _gameState.stock_market.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                _highlightedStock = Math.Clamp(_highlightedStock + 1, 0, _gameState.stock_market.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                SellStock();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
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

            if (_gameState.stock_market[_highlightedStock].current_quantity == 0 || _gameState.cmdr.credits < _gameState.stock_market[_highlightedStock].current_price)
            {
                return;
            }

            if (_gameState.stock_market[_highlightedStock].units == GameState.TONNES && _trade.total_cargo() == _ship.cargoCapacity)
            {
                return;
            }

            _gameState.cmdr.current_cargo[_highlightedStock]++;
            _gameState.stock_market[_highlightedStock].current_quantity--;
            _gameState.cmdr.credits -= _gameState.stock_market[_highlightedStock].current_price;
        }

        private void SellStock()
        {
            if (!elite.docked)
            {
                return;
            }

            if (_gameState.cmdr.current_cargo[_highlightedStock] == 0)
            {
                return;
            }

            _gameState.cmdr.current_cargo[_highlightedStock]--;
            _gameState.stock_market[_highlightedStock].current_quantity++;
            _gameState.cmdr.credits += _gameState.stock_market[_highlightedStock].current_price;
        }
    }
}