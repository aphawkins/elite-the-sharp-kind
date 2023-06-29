// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Views;

namespace EliteSharp.Planets
{
    internal static class PlanetFactory
    {
        internal static IPlanetRenderer Create(PlanetType type, IGraphics graphics, IDraw draw, int seed) => type switch
        {
            PlanetType.Fractal => new FractalPlanet(graphics, draw, seed),
            PlanetType.Wireframe => new WireframePlanet(graphics, draw),
            PlanetType.Green => new GreenPlanet(graphics, draw),
            PlanetType.SNES => new SnesPlanet(graphics, draw),
            _ => throw new NotImplementedException(),
        };
    }
}
