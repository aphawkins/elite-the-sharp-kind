// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views.Planets;

/// <summary>
/// Draws a planet, once the game has worked out where on screen it is and how
/// big. The planet itself belongs to the universe - it has a position, it is
/// cloned, it moves - and none of that is a rendition's business; what a
/// rendition decides is what the thing looks like when it gets there.
/// <para>
/// One of these per style per rendition, so a rendition is free to draw its
/// striped planet nothing like the other's rather than being limited to
/// choosing different colours for the same drawing.
/// </para>
/// </summary>
public interface IPlanetRenderer
{
    /// <summary>
    /// Draws the planet.
    /// </summary>
    /// <param name="planet">Where it is on screen, how big, and which way up.</param>
    public void Draw(PlanetView planet);
}
