// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Planets
{
    internal static class PlanetFactory
    {
        internal static IObject Create(PlanetType type, IDraw draw, int seed) => type switch
        {
            PlanetType.Fractal => new FractalPlanet(draw, seed),
            PlanetType.Wireframe => new WireframePlanet(draw),
            PlanetType.Solid => new SolidPlanet(draw, EliteColors.Green),
            PlanetType.Striped => new StripedPlanet(draw),
            _ => throw new EliteException(),
        };
    }
}
