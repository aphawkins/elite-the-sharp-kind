// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Planets;

namespace EliteSharp.Ships
{
    internal sealed class Planet : ShipBase
    {
        internal Planet(IPlanetRenderer renderer)
        {
            PlanetRenderer = renderer;
            Type = ShipType.Planet;
        }

        internal IPlanetRenderer PlanetRenderer { get; }
    }
}
