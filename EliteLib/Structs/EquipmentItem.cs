namespace Elite.Structs
{
    using Elite.Enums;

    internal class EquipmentItem
    {
        internal bool CanBuy;
        internal bool Show;
        internal int TechLevel;
        internal int Price;
        internal string Name;
        internal EquipmentType Type;

        internal EquipmentItem(bool canBuy, bool show, int techLevel, int price, string name, EquipmentType type)
        {
            CanBuy = canBuy;
            Show = show;
            TechLevel = techLevel;
            Price = price;
            Name = name;
            Type = type;
        }
    };
}
