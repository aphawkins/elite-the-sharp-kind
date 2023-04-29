namespace Elite.Engine
{
    internal class StockItem
    {
        internal string name;
        internal int currentQuantity;
        internal float currentPrice;
        internal float basePrice;
        internal int economyAdjust;
        internal int baseQuantity;
        internal int mask;
        internal string units;
        internal int stationStock;
        internal int currentCargo;

        internal StockItem(string name, int currentQuantity, float currentPrice, float basePrice,
            int economyAdjust, int baseQuantity, int mask, string units,
            int stationStock, int currentCargo)
        {
            this.name = name;
            this.currentQuantity = currentQuantity;
            this.currentPrice = currentPrice;
            this.basePrice = basePrice;
            this.economyAdjust = economyAdjust;
            this.baseQuantity = baseQuantity;
            this.mask = mask;
            this.units = units;
            this.stationStock = stationStock;
            this.currentCargo = currentCargo;
        }
    };
}