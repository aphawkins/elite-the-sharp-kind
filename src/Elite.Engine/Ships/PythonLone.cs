// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Ships
{
    internal sealed class PythonLone : Python
    {
        internal PythonLone()
        {
            Type = ShipType.PythonLone;
            Flags = ShipFlags.Bold | ShipFlags.Angry;
            Bounty = 20;
            LootMax = 2;
            Class = ShipClass.LoneWolf;
        }
    }
}
