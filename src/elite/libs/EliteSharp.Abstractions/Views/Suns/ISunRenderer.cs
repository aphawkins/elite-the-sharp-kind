// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// Draws a sun, once the game has worked out where on screen it is and how
/// big. As with a planet, the sun itself stays in the universe and only its
/// appearance is the rendition's.
/// </summary>
public interface ISunRenderer
{
    /// <summary>
    /// Draws the sun.
    /// </summary>
    /// <param name="sun">Where it is on screen, and how big.</param>
    public void Draw(SunView sun);
}
