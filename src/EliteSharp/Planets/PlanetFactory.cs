// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Planets
{
    internal static class PlanetFactory
    {
        internal static IPlanetRenderer Create(PlanetType type, IDraw draw, int seed) => type switch
        {
            PlanetType.Fractal => new FractalPlanet(draw, seed),
            PlanetType.Wireframe => new WireframePlanet(draw),
            PlanetType.Green => new GreenPlanet(draw),
            PlanetType.SNES => new SnesPlanet(draw),
            _ => throw new NotImplementedException(),
        };
    }
}
