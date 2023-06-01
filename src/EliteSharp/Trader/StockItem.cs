// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Trader
{
    internal sealed class StockItem
    {
        internal StockItem(
            string name,
            int currentQuantity,
            float currentPrice,
            float basePrice,
            int economyAdjust,
            int baseQuantity,
            int mask,
            string units,
            int stationStock,
            int currentCargo)
        {
            Name = name;
            CurrentQuantity = currentQuantity;
            CurrentPrice = currentPrice;
            BasePrice = basePrice;
            EconomyAdjust = economyAdjust;
            BaseQuantity = baseQuantity;
            Mask = mask;
            Units = units;
            StationStock = stationStock;
            CurrentCargo = currentCargo;
        }

        internal float BasePrice { get; set; }

        internal int BaseQuantity { get; set; }

        internal int CurrentCargo { get; set; }

        internal float CurrentPrice { get; set; }

        internal int CurrentQuantity { get; set; }

        internal int EconomyAdjust { get; set; }

        internal int Mask { get; set; }

        internal string Name { get; set; }

        internal int StationStock { get; set; }

        internal string Units { get; set; }
    }
}
