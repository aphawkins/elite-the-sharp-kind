namespace Elite.Engine
{
    using Elite.Engine.Ships;
    using Elite.Engine.Types;

    internal class Trade
    {
        internal const string GRAMS = "g";
        internal const string KILOGRAMS = "Kg";
        internal const string TONNES = "t";

        internal float credits;

        internal int marketRandomiser;

        /// <summary>
        /// The following holds the Elite Planet Stock Market.
        /// </summary>
        internal Dictionary<StockType, stock_item> stock_market = new()
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

        private readonly PlayerShip _ship;

        internal Trade(PlayerShip ship)
        {
            _ship = ship;
        }

        internal void AddCargo(StockType stock)
        {
            stock_market[stock].currentCargo++;
        }

        internal void BuyStock(StockType stock)
        {
            if (stock_market[stock].current_quantity == 0 || credits < stock_market[stock].current_price)
            {
                return;
            }

            if (stock_market[stock].units == Trade.TONNES && TotalCargoTonnage() == _ship.cargoCapacity)
            {
                return;
            }

            stock_market[stock].currentCargo++;
            stock_market[stock].current_quantity--;
            credits -= stock_market[stock].current_price;
        }

        internal void ClearCurrentCargo()
        {
            foreach (var stock in stock_market)
            {
                stock.Value.currentCargo = 0;
            }
        }

        /// <summary>
        /// Generate the Elite stock market.
        /// The prices and quantities are affected by the planet's economy.
        /// There is also a slight amount of randomness added in.
        /// The random value is changed each time we hyperspace.
        /// </summary>
        internal void GenerateStockMarket(planet_data currentPlanet)
        {
            foreach (var stock in stock_market)
            {
                // Start with the base price
                float price = stock.Value.base_price;
                // Add in a random amount
                price += (marketRandomiser & stock.Value.mask) / 10;
                // Adjust for planet economy
                price += currentPlanet.economy * stock.Value.eco_adjust / 10;

                // Start with the base quantity
                int quant = stock.Value.base_quantity;
                // Add in a random amount
                quant += marketRandomiser & stock.Value.mask;
                // Adjust for planet economy
                quant -= currentPlanet.economy * stock.Value.eco_adjust;
                // Quantities range from 0..63
                quant = Math.Clamp(quant, 0, 63);

                stock.Value.current_price = price * 4;
                stock.Value.current_quantity = quant;
            }

            // Alien Items are never available for purchase
            stock_market[StockType.AlienItems].current_quantity = 0;
        }

        internal int IsCarryingContraband() => ((stock_market[StockType.Slaves].currentCargo + stock_market[StockType.Slaves].currentCargo) * 2) + stock_market[StockType.Firearms].currentCargo;

        internal void SellStock(StockType stock)
        {
            if (stock_market[stock].currentCargo == 0)
            {
                return;
            }

            stock_market[stock].currentCargo--;
            stock_market[stock].current_quantity++;
            credits += stock_market[stock].current_price;
        }

        internal void SetStockQuantities()
        {
            foreach (var stock in stock_market)
            {
                stock.Value.current_quantity = stock.Value.stationStock;
            }

            // Alien Items are never available for purchase
            stock_market[StockType.AlienItems].current_quantity = 0;
        }

        internal int TotalCargoTonnage()
        {
            int cargo = 0;

            foreach (var stock in stock_market)
            {
                if ((stock.Value.currentCargo > 0) && (stock.Value.units == TONNES))
                {
                    cargo += stock.Value.currentCargo;
                }
            }

            return cargo;
        }
    }
}