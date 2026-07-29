// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Views;

/// <summary>
/// The inventory screen: what the ship is carrying, formatted for display.
/// </summary>
internal sealed class InventoryController : IScreenController
{
    private readonly PlayerShip _ship;
    private readonly Trade _trade;
    private readonly IView<InventoryModel> _view;

    internal InventoryController(PlayerShip ship, Trade trade, IView<InventoryModel> view)
    {
        _ship = ship;
        _trade = trade;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

    public void HandleInput()
    {
    }

    public void Reset()
    {
    }

    public void Update()
    {
    }

    // Exposed for tests: only stock with cargo aboard appears.
    internal InventoryModel BuildModel() => new(
        "INVENTORY",
        $"{_ship.Fuel:N1} Light Years",
        $"{_trade.Credits:N1} Credits",
        [.. _trade.StockMarket
            .Where(stock => stock.Value.CurrentCargo > 0)
            .Select(stock => (stock.Value.Name, $"{stock.Value.CurrentCargo}{stock.Value.Units}"))]);
}
