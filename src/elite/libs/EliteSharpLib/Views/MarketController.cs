// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Trader;
using Useful.Controls;

namespace EliteSharpLib.Views;

/// <summary>
/// The market screen's behaviour: the cursor over the stock list, and buying
/// or selling whichever stock it is on.
/// </summary>
internal sealed class MarketController : IScreenController
{
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlanetController _planet;
    private readonly Trade _trade;
    private readonly IView<MarketModel> _view;

    private StockType _highlightedStock;

    internal MarketController(GameState gameState, IKeyboard keyboard, Trade trade, PlanetController planet, IView<MarketModel> view)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _trade = trade;
        _planet = planet;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.S) || _keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            // StockType runs 1 (Food) to Count (AlienItems), not 0-based.
            _highlightedStock = (StockType)Math.Clamp((int)_highlightedStock - 1, 1, _trade.StockMarket.Count);
        }

        if (_keyboard.IsPressed(ConsoleKey.X) || _keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            _highlightedStock = (StockType)Math.Clamp((int)_highlightedStock + 1, 1, _trade.StockMarket.Count);
        }

        if (_keyboard.IsPressed(ConsoleKey.OemComma) || _keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            _trade.SellStock(_highlightedStock);
        }

        if (_keyboard.IsPressed(ConsoleKey.OemPeriod) || _keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            _trade.BuyStock(_highlightedStock);
        }
    }

    public void Reset() => _highlightedStock = StockType.Food;

    public void Update()
    {
    }

    // Exposed for tests: the stock rows and which one the cursor is on.
    internal MarketModel BuildModel()
    {
        List<MarketRow> rows = [];
        foreach (KeyValuePair<StockType, StockItem> stock in _trade.StockMarket)
        {
            rows.Add(new(
                stock.Value.Name,
                stock.Value.Units,
                stock.Value.CurrentPrice,
                stock.Value.CurrentQuantity,
                stock.Value.CurrentCargo,
                stock.Key == _highlightedStock));
        }

        return new($"{_planet.NamePlanet(_gameState.DockedPlanet)} MARKET PRICES", rows, _trade.Credits);
    }
}
