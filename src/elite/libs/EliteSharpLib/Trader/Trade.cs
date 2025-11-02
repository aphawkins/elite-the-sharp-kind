// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
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

    internal Trade(GameState gameState, PlayerShip ship)
    {
        _gameState = gameState;
        _ship = ship;
    }

    internal float Credits { get; set; }

    internal int MarketRandomiser { get; set; }

    internal Dictionary<StockType, StockItem> StockMarket { get; } = new()
    {
        { StockType.Food,         new("Food",          0, 0,  1.9f, -2,   6,   1, TONNES,    0, 0) },
        { StockType.Textiles,     new("Textiles",      0, 0,  2.0f, -1,  10,   3, TONNES,    0, 0) },
        { StockType.Radioactives, new("Radioactives",  0, 0,  6.5f, -3,   2,   7, TONNES,    0, 0) },
        { StockType.Slaves,       new("Slaves",        0, 0,  4.0f, -5, 226,  31, TONNES,    0, 0) },
        { StockType.LiquorWines,  new("Liquor/Wines",  0, 0,  8.3f, -5, 251,  15, TONNES,    0, 0) },
        { StockType.Luxuries,     new("Luxuries",      0, 0, 19.6f,  8,  54,   3, TONNES,    0, 0) },
        { StockType.Narcotics,    new("Narcotics",     0, 0, 23.5f, 29,   8, 120, TONNES,    0, 0) },
        { StockType.Computers,    new("Computers",     0, 0, 15.4f, 14,  56,   3, TONNES,    0, 0) },
        { StockType.Machinery,    new("Machinery",     0, 0, 11.7f,  6,  40,   7, TONNES,    0, 0) },
        { StockType.Alloys,       new("Alloys",        0, 0,  7.8f,  1,  17,  31, TONNES,    0, 0) },
        { StockType.Firearms,     new("Firearms",      0, 0, 12.4f, 13,  29,   7, TONNES,    0, 0) },
        { StockType.Furs,         new("Furs",          0, 0, 17.6f, -9, 220,  63, TONNES,    0, 0) },
        { StockType.Minerals,     new("Minerals",      0, 0,  3.2f, -1,  53,   3, TONNES,    0, 0) },
        { StockType.Gold,         new("Gold",          0, 0,  9.7f, -1,  66,   7, KILOGRAMS, 0, 0) },
        { StockType.Platinum,     new("Platinum",      0, 0, 17.1f, -2,  55,  31, KILOGRAMS, 0, 0) },
        { StockType.GemStones,    new("Gem-Stones",    0, 0,  4.5f, -1, 250,  15, GRAMS,     0, 0) },
        { StockType.AlienItems,   new("Alien Items",   0, 0,  5.3f, 15, 192,   7, TONNES,    0, 0) },
    };

    internal void AddCargo(StockType stock) => StockMarket[stock].CurrentCargo++;

    internal void BuyStock(StockType stock)
    {
        if (StockMarket[stock].CurrentQuantity == 0 || Credits < StockMarket[stock].CurrentPrice)
        {
            return;
        }

        if (StockMarket[stock].Units == TONNES && TotalCargoTonnage() == _ship.CargoCapacity)
        {
            return;
        }

        StockMarket[stock].CurrentCargo++;
        StockMarket[stock].CurrentQuantity--;
        Credits -= StockMarket[stock].CurrentPrice;
    }

    internal void ClearCurrentCargo()
    {
        foreach (KeyValuePair<StockType, StockItem> stock in StockMarket)
        {
            stock.Value.CurrentCargo = 0;
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
        foreach (KeyValuePair<StockType, StockItem> stock in StockMarket)
        {
            // Start with the base price
            float price = stock.Value.BasePrice;

            // Add in a random amount
            price += (MarketRandomiser & stock.Value.Mask) / 10f;

            // Adjust for planet economy
            price += _gameState.CurrentPlanetData.Economy * stock.Value.EconomyAdjust / 10f;

            // Start with the base quantity
            int quant = stock.Value.BaseQuantity;

            // Add in a random amount
            quant += MarketRandomiser & stock.Value.Mask;

            // Adjust for planet economy
            quant -= _gameState.CurrentPlanetData.Economy * stock.Value.EconomyAdjust;

            // Quantities range from 0..63
            quant = Math.Clamp(quant, 0, 63);

            stock.Value.CurrentPrice = price * 4;
            stock.Value.CurrentQuantity = quant;
        }

        // Alien Items are never available for purchase
        StockMarket[StockType.AlienItems].CurrentQuantity = 0;
    }

    internal int IsCarryingContraband() => ((StockMarket[StockType.Slaves].CurrentCargo + StockMarket[StockType.Slaves].CurrentCargo)
        * 2)
        + StockMarket[StockType.Firearms].CurrentCargo;

    internal void SellStock(StockType stock)
    {
        if (StockMarket[stock].CurrentCargo == 0)
        {
            return;
        }

        StockMarket[stock].CurrentCargo--;
        StockMarket[stock].CurrentQuantity++;
        Credits += StockMarket[stock].CurrentPrice;
    }

    internal void SetStockQuantities()
    {
        foreach (KeyValuePair<StockType, StockItem> stock in StockMarket)
        {
            stock.Value.CurrentQuantity = stock.Value.StationStock;
        }

        // Alien Items are never available for purchase
        StockMarket[StockType.AlienItems].CurrentQuantity = 0;
    }

    internal int TotalCargoTonnage()
    {
        int cargo = 0;

        foreach (KeyValuePair<StockType, StockItem> stock in StockMarket)
        {
            if (stock.Value.CurrentCargo > 0 && stock.Value.Units == TONNES)
            {
                cargo += stock.Value.CurrentCargo;
            }
        }

        return cargo;
    }
}
