// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Ships;
using Elite.Engine.Types;

namespace Elite.Engine
{
    internal sealed class Universe
    {
        internal Dictionary<ShipType, int> _shipCount = new();
        internal UniverseObject[] _universe = new UniverseObject[EliteMain.MaxUniverseObjects];
    }
}
