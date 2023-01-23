namespace Elite.Structs
{
    internal class stock_item
    {
        internal string name;
        internal int current_quantity;
        internal float current_price;
        internal float base_price;
        internal int eco_adjust;
        internal int base_quantity;
        internal int mask;
        internal string units;

        internal stock_item(string name, int current_quantity, float current_price, float base_price, int eco_adjust, int base_quantity, int mask, string units)
        {
            this.name = name;
            this.current_quantity = current_quantity;
            this.current_price = current_price;
            this.base_price = base_price;
            this.eco_adjust = eco_adjust;
            this.base_quantity = base_quantity;
            this.mask = mask;
            this.units = units;
        }
    };
}