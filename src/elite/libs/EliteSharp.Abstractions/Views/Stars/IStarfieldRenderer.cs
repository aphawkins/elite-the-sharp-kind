// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views.Stars;

/// <summary>
/// Draws the starfield. The stars themselves are the game's: it moves them,
/// rolls them with the ship and recycles them off the view's edges, and hands
/// over only what is to be drawn where.
/// </summary>
public interface IStarfieldRenderer
{
    /// <summary>
    /// Draws every star the game is showing this frame.
    /// </summary>
    /// <param name="stars">The stars, already in screen coordinates.</param>
    public void Draw(IReadOnlyList<StarMark> stars);
}
