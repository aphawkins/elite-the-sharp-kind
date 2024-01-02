// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Equipment;

namespace EliteSharp.Types
{
    internal sealed class EquipmentItem
    {
        internal EquipmentItem(bool canBuy, bool show, int techLevel, float price, string name, EquipmentType type)
        {
            CanBuy = canBuy;
            Show = show;
            TechLevel = techLevel;
            Price = price;
            Name = name;
            Type = type;
        }

        internal bool CanBuy { get; set; }

        internal string Name { get; set; }

        internal float Price { get; set; }

        internal bool Show { get; set; }

        internal int TechLevel { get; set; }

        internal EquipmentType Type { get; set; }
    }
}
