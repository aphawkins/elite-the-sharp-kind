// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Planets;

internal static class PlanetFactory
{
    // A wireframe world overrides the planet style; the original picks a
    // crater or an equator-and-meridian from bit 1 of the system's tech
    // level (SOS1).
    internal static IObject Create(GraphicStyle style, PlanetType type, IEliteDraw draw, int seed, int techLevel)
        => style == GraphicStyle.Wireframe
            ? new WireframePlanet(draw, (techLevel & 2) != 0)
            : type switch
            {
                PlanetType.Fractal => new FractalPlanet(draw, seed),
                PlanetType.Solid => new SolidPlanet(draw),
                PlanetType.Striped => new StripedPlanet(draw),
                _ => throw new EliteException(),
            };
}
