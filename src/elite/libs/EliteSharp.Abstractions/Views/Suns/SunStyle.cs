// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// How a sun is drawn. The commander picks it in the settings and a wireframe
/// world overrides it, so which one applies is the game's decision - what each
/// looks like is the rendition's.
/// </summary>
public enum SunStyle
{
    /// <summary>
    /// An outline only.
    /// </summary>
    Wireframe = 0,

    /// <summary>
    /// A flat disc of one colour.
    /// </summary>
    Solid = 1,

    /// <summary>
    /// Banded from a white core out through a flaring rim.
    /// </summary>
    Gradient = 2,
}
