// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal class PythonLone : Python, IShip
    {
        public new float Bounty => 20;
        public new int LootMax => 2;
        public new ShipClass Type => ShipClass.LoneWolf;
    }
}
