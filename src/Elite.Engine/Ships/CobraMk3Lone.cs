// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Ships
{
    internal sealed class CobraMk3Lone : CobraMk3
    {
        internal CobraMk3Lone()
        {
            Type = ShipType.CobraMk3Lone;
            Flags = ShipFlags.Bold | ShipFlags.Angry;
            Bounty = 17.5f;
            LootMax = 1;
            MissilesMax = 2;
        }
    }
}
