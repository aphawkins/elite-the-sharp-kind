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
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly PlanetController _planet;
        private readonly Trade _trade;
        private StockType _highlightedStock;

        internal MarketView(GameState gameState, IGraphics graphics, Draw draw, IKeyboard keyboard, Trade trade, PlanetController planet)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
            _trade = trade;
            _planet = planet;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader($"{_planet.NamePlanet(_gameState.DockedPlanet)} MARKET PRICES");

            _graphics.DrawTextLeft(16, 40, "PRODUCT", Colour.Green);
            _graphics.DrawTextLeft(166, 40, "UNIT", Colour.Green);
            _graphics.DrawTextLeft(246, 40, "PRICE", Colour.Green);
            _graphics.DrawTextLeft(314, 40, "FOR SALE", Colour.Green);
            _graphics.DrawTextLeft(420, 40, "IN HOLD", Colour.Green);

            int i = 0;
            foreach (KeyValuePair<StockType, StockItem> stock in _trade.StockMarket)
            {
                int y = (i * 15) + 55;

                if (stock.Key == _highlightedStock)
                {
                    _graphics.DrawRectangleFilled(2, y, 508, 15, Colour.LightRed);
                }

                _graphics.DrawTextLeft(16, y, stock.Value.Name, Colour.White);

                _graphics.DrawTextLeft(180, y, stock.Value.Units, Colour.White);

                _graphics.DrawTextRight(285, y, $"{stock.Value.CurrentPrice:N1}", Colour.White);

                _graphics.DrawTextRight(365, y, stock.Value.CurrentQuantity > 0 ? $"{stock.Value.CurrentQuantity}" : "-", Colour.White);
                _graphics.DrawTextLeft(365, y, stock.Value.CurrentQuantity > 0 ? stock.Value.Units : string.Empty, Colour.White);

                _graphics.DrawTextRight(455, y, stock.Value.CurrentCargo > 0 ? $"{stock.Value.CurrentCargo,2}" : "-", Colour.White);
                _graphics.DrawTextLeft(455, y, stock.Value.CurrentCargo > 0 ? stock.Value.Units : string.Empty, Colour.White);

                i++;
            }

            _graphics.DrawTextLeft(16, 340, "Cash:", Colour.Green);
            _graphics.DrawTextRight(160, 340, $"{_trade.Credits,10:N1} Credits", Colour.White);
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
