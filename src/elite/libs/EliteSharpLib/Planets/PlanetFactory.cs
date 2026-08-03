// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Renditions;
using EliteSharp.Abstractions.Views.Planets;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Planets;

internal static class PlanetFactory
{
    // A wireframe world overrides the planet style; the original picks a
    // crater or an equator-and-meridian from bit 1 of the system's tech
    // level (SOS1).
    //
    // Which style applies stays the game's decision - the commander chose it
    // in the settings - and what that style looks like is the rendition's.
    // This no longer knows which renditions exist.
    internal static IObject Create(
        GraphicStyle style,
        PlanetType type,
        IEliteDraw draw,
        IRendition rendition,
        int seed,
        int techLevel)
    {
        ArgumentNullException.ThrowIfNull(rendition);

        PlanetStyle planetStyle = style == GraphicStyle.Wireframe
            ? PlanetStyle.Wireframe
            : type switch
            {
                PlanetType.Fractal => PlanetStyle.Fractal,
                PlanetType.Solid => PlanetStyle.Solid,
                PlanetType.Striped => PlanetStyle.Striped,
                _ => throw new EliteException(),
            };

        // Reference: fesh0r/newkind's generate_fractal_landscape(rnd_seed)
        // reseeds a single stream for the whole landscape, so the same system
        // always renders the same planet. The game owns that stream, as it
        // owns every other one.
        Random random = new(seed);
        PlanetLook look = new(planetStyle, (techLevel & 2) != 0, new RandomSource(random));

        return new Planet(draw, rendition.CreatePlanetRenderer(draw, look), planetStyle == PlanetStyle.Wireframe);
    }
}
