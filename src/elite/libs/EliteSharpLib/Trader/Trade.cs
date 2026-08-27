// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;

namespace EliteSharpLib.Trader;

internal sealed class Trade
{
    internal const string GRAMS = "g";
    internal const string KILOGRAMS = "Kg";
    internal const string TONNES = "t";

    private readonly GameState _gameState;

    private readonly PlayerShip _ship;

    private readonly Dictionary<string, StockItem> _byId;

    internal Trade(GameState gameState, PlayerShip ship)
    {
        _gameState = gameState;
        _ship = ship;

        StockMarket = [.. ClassicGoods.All.Select(definition => new StockItem(definition))];
        _byId = StockMarket.ToDictionary(stock => stock.Definition.Id, StringComparer.Ordinal);
        DroppedByShips = [.. StockMarket.Where(stock => stock.Definition.IsDroppedByShips)];
    }

    internal float Credits { get; set; }

    internal int MarketRandomiser { get; set; }

    /// <summary>
    /// Gets every good, in the order the market screen lists them. An ordered
    /// list rather than a dictionary because that order is shown to the
    /// commander and saved to file, and a dictionary only happens to keep it.
    /// </summary>
    internal IReadOnlyList<StockItem> StockMarket { get; }

    /// <summary>
    /// Gets the goods a cargo canister can hold, in the same order. Held rather
    /// than filtered each time because a canister is opened mid-combat and the
    /// answer never changes.
    /// </summary>
    internal IReadOnlyList<StockItem> DroppedByShips { get; }

    /// <summary>
    /// The good that goes by this name.
    /// </summary>
    /// <param name="id">The good's <see cref="GoodsDefinition.Id"/>.</param>
    /// <returns>That good's current standing.</returns>
    internal StockItem this[string id] => _byId[id];

    internal void BuyStock(StockItem stock)
    {
        ArgumentNullException.ThrowIfNull(stock);

        if (stock.CurrentQuantity == 0 || Credits < stock.CurrentPrice)
        {
            return;
        }

        if (stock.Definition.FillsHold && TotalCargoTonnage() == _ship.CargoCapacity)
        {
            return;
        }

        stock.CurrentCargo++;
        stock.CurrentQuantity--;
        Credits -= stock.CurrentPrice;
    }

    internal void ClearCurrentCargo()
    {
        foreach (StockItem stock in StockMarket)
        {
            stock.CurrentCargo = 0;
        }
    }

    /// <summary>
    /// Generate the Elite stock market.
    /// The prices and quantities are affected by the planet's economy.
    /// There is also a slight amount of randomness added in.
    /// The random value is changed each time we hyperspace.
    /// </summary>
    internal void GenerateStockMarket()
    {
        foreach (StockItem stock in StockMarket)
        {
            GoodsDefinition definition = stock.Definition;

            // Start with the base price
            float price = definition.BasePrice;

            // Add in a random amount
            price += (MarketRandomiser & definition.Mask) / 10f;

            // Adjust for planet economy
            price += _gameState.CurrentPlanetData.Economy * definition.EconomyAdjust / 10f;

            // Start with the base quantity
            int quant = definition.BaseQuantity;

            // Add in a random amount
            quant += MarketRandomiser & definition.Mask;

            // Adjust for planet economy
            quant -= _gameState.CurrentPlanetData.Economy * definition.EconomyAdjust;

            // Quantities range from 0..63
            quant = Math.Clamp(quant, 0, 63);

            stock.CurrentPrice = price * 4;
            stock.CurrentQuantity = definition.IsSoldByStations ? quant : 0;
        }
    }

    internal int IsCarryingContraband()
        => StockMarket.Sum(stock => stock.CurrentCargo * stock.Definition.ContrabandWeight);

    internal void SellStock(StockItem stock)
    {
        ArgumentNullException.ThrowIfNull(stock);

        if (stock.CurrentCargo == 0)
        {
            return;
        }

        stock.CurrentCargo--;
        stock.CurrentQuantity++;
        Credits += stock.CurrentPrice;
    }

    internal void SetStockQuantities()
    {
        foreach (StockItem stock in StockMarket)
        {
            stock.CurrentQuantity = stock.Definition.IsSoldByStations ? stock.StationStock : 0;
        }
    }

    internal int TotalCargoTonnage()
    {
        int cargo = 0;

        foreach (StockItem stock in StockMarket)
        {
            if (stock.CurrentCargo > 0 && stock.Definition.FillsHold)
            {
                cargo += stock.CurrentCargo;
            }
        }

        return cargo;
    }
}
