// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Planets
{
    internal static class PlanetFactory
    {
        internal static IPlanetRenderer Create(PlanetType type, IGraphics graphics, int seed) => type switch
        {
            PlanetType.Fractal => new FractalPlanet(graphics, seed),
            PlanetType.Wireframe => new WireframePlanet(graphics),
            PlanetType.Green => new GreenPlanet(graphics),
            PlanetType.SNES => new SnesPlanet(graphics),
            _ => throw new NotImplementedException(),
        };
    }
}
