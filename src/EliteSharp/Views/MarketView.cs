// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Trader;

namespace EliteSharp.Views
{
    internal sealed class MarketView : IView
    {
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IKeyboard _keyboard;
        private readonly PlanetController _planet;
        private readonly Trade _trade;
        private StockType _highlightedStock;

        internal MarketView(GameState gameState, IDraw draw, IKeyboard keyboard, Trade trade, PlanetController planet)
        {
            _gameState = gameState;
            _draw = draw;
            _keyboard = keyboard;
            _trade = trade;
            _planet = planet;
        }

        public void Draw()
        {
            _draw.DrawViewHeader($"{_planet.NamePlanet(_gameState.DockedPlanet)} MARKET PRICES");

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 40), "PRODUCT", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(166 + _draw.Offset, 40), "UNIT", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(246 + _draw.Offset, 40), "PRICE", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(314 + _draw.Offset, 40), "FOR SALE", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(420 + _draw.Offset, 40), "IN HOLD", EliteColors.Green);

            int i = 0;
            foreach (KeyValuePair<StockType, StockItem> stock in _trade.StockMarket)
            {
                int y = (i * 15) + 55;

                if (stock.Key == _highlightedStock)
                {
                    _draw.Graphics.DrawRectangleFilled(new(2 + _draw.Offset, y), 508, 15, EliteColors.LightRed);
                }

                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, y), stock.Value.Name, EliteColors.White);

                _draw.Graphics.DrawTextLeft(new(180 + _draw.Offset, y), stock.Value.Units, EliteColors.White);

                _draw.Graphics.DrawTextRight(new(285 + _draw.Offset, y), $"{stock.Value.CurrentPrice:N1}", EliteColors.White);

                _draw.Graphics.DrawTextRight(
                    new(365 + _draw.Offset, y),
                    stock.Value.CurrentQuantity > 0 ? $"{stock.Value.CurrentQuantity}" : "-",
                    EliteColors.White);
                _draw.Graphics.DrawTextLeft(
                    new(365 + _draw.Offset, y),
                    stock.Value.CurrentQuantity > 0 ? stock.Value.Units : string.Empty,
                    EliteColors.White);

                _draw.Graphics.DrawTextRight(
                    new(455 + _draw.Offset, y),
                    stock.Value.CurrentCargo > 0 ? $"{stock.Value.CurrentCargo,2}" : "-",
                    EliteColors.White);
                _draw.Graphics.DrawTextLeft(
                    new(455 + _draw.Offset, y),
                    stock.Value.CurrentCargo > 0 ? stock.Value.Units : string.Empty,
                    EliteColors.White);

                i++;
            }

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 340), "Cash:", EliteColors.Green);
            _draw.Graphics.DrawTextRight(new(225 + _draw.Offset, 340), $"{_trade.Credits,10:N1} Credits", EliteColors.White);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                _highlightedStock = (StockType)Math.Clamp((int)_highlightedStock - 1, 0, _trade.StockMarket.Count - 1);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                _highlightedStock = (StockType)Math.Clamp((int)_highlightedStock + 1, 0, _trade.StockMarket.Count - 1);
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

        public void Reset() => _highlightedStock = 0;

        public void UpdateUniverse()
        {
        }
    }
}
