// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class PythonLone : Python
    {
        internal PythonLone(IDraw draw)
            : base(draw)
        {
            Type = ShipType.PythonLone;
            Flags = ShipFlags.LoneWolf | ShipFlags.Bold | ShipFlags.Angry;
            Bounty = 20;
            LootMax = 2;
        }
    }
}
