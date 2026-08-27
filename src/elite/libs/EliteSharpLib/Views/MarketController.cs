// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Trader;
using SharpKind.Input;

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

    private int _highlightedRow;

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
            // A row in the list rather than a good's number: the goods are a
            // set the game is given now, and what they are called says nothing
            // about what order they come in.
            _highlightedRow = Math.Clamp(_highlightedRow - 1, 0, _trade.StockMarket.Count - 1);
        }

        if (_keyboard.IsPressed(ConsoleKey.X) || _keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            _highlightedRow = Math.Clamp(_highlightedRow + 1, 0, _trade.StockMarket.Count - 1);
        }

        if (_keyboard.IsPressed(ConsoleKey.OemComma) || _keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            _trade.SellStock(_trade.StockMarket[_highlightedRow]);
        }

        if (_keyboard.IsPressed(ConsoleKey.OemPeriod) || _keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            _trade.BuyStock(_trade.StockMarket[_highlightedRow]);
        }
    }

    public void Reset() => _highlightedRow = 0;

    public void Update()
    {
    }

    // Exposed for tests: the stock rows and which one the cursor is on.
    internal MarketModel BuildModel()
    {
        List<MarketRow> rows = [];
        for (int row = 0; row < _trade.StockMarket.Count; row++)
        {
            StockItem stock = _trade.StockMarket[row];
            rows.Add(new(
                stock.Definition.Name,
                stock.Definition.Units,
                stock.CurrentPrice,
                stock.CurrentQuantity,
                stock.CurrentCargo,
                row == _highlightedRow));
        }

        return new($"{_planet.NamePlanet(_gameState.DockedPlanet)} MARKET PRICES", rows, _trade.Credits);
    }
}
