// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views.Planets;

/// <summary>
/// How a planet is drawn. The commander picks it in the settings and the
/// wireframe world overrides it, so which one applies is the game's decision -
/// what each looks like is the rendition's.
/// </summary>
public enum PlanetStyle
{
    /// <summary>
    /// An outline, with a crater or an equator and meridian.
    /// </summary>
    Wireframe = 0,

    /// <summary>
    /// A flat disc of one colour.
    /// </summary>
    Solid = 1,

    /// <summary>
    /// Banded, pole to pole.
    /// </summary>
    Striped = 2,

    /// <summary>
    /// A generated surface of land and sea.
    /// </summary>
    Fractal = 3,
}
