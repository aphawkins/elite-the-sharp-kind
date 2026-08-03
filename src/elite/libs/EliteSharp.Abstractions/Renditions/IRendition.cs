// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;

namespace EliteSharp.Abstractions.Renditions;

/// <summary>
/// Everything one rendition of the game draws, written in its own assembly
/// and found at startup - the same door the missions come through. A whole
/// presentation is then an assembly rather than a branch in the game.s
/// composition root.
/// <para>
/// A rendition holds no state: it is handed an <see cref="IViewSurface"/> and
/// builds views on it, and the game keeps what it built. Unlike a mission, a
/// rendition is not optional - the tier the commander configured has to be
/// installed or there is nothing to draw with.
/// </para>
/// </summary>
public interface IRendition
{
    /// <summary>
    /// Gets the name this rendition is known by - in the config file, and in
    /// the folder its assets sit in. It has to stay put across releases: a
    /// renamed rendition is one the commander's config no longer selects.
    /// <para>
    /// It is a name rather than one of a fixed set, because the game cannot
    /// know what renditions exist. The two it ships with stand in for 8-bit
    /// and 16-bit machines; a third need not be a machine at all.
    /// </para>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the width in pixels this rendition draws at. The game renders at
    /// this size and the window magnifies it, so a rendition picks its own
    /// resolution rather than being handed one.
    /// </summary>
    public int ScreenWidth { get; }

    /// <summary>
    /// Gets the height in pixels this rendition draws at.
    /// </summary>
    public int ScreenHeight { get; }

    /// <summary>
    /// Gets the coordinate scale: the original's drawing maths is written in a
    /// 256-square space and multiplied up to the render resolution, so a
    /// rendition twice the original's size uses 2. Whole numbers only - a
    /// fraction puts HUD text and ship vertices on half-pixels.
    /// </summary>
    public int Scale { get; }

    /// <summary>
    /// Builds the chrome every screen of this tier shares - the border, the
    /// header, the countdown, the word wrap.
    /// </summary>
    public IBaseView CreateBaseView(IViewSurface surface);

    /// <summary>
    /// Builds the mission briefing screen, which is the one screen that tells
    /// the game something back: where this tier puts the ship posing behind a
    /// briefing.
    /// </summary>
    public IMissionBriefingView CreateMissionBriefingView(IViewSurface surface);

    /// <summary>
    /// Builds every other screen. The game checks the set holds all of them
    /// before it draws a frame.
    /// </summary>
    public ViewSet CreateViews(IViewSurface surface);

    /// <summary>
    /// Builds a renderer for one planet. Planets live in the universe rather
    /// than on a screen - they move, they are cloned - so the game keeps the
    /// planet and the rendition supplies only what it looks like.
    /// </summary>
    /// <param name="surface">What the renderer draws on.</param>
    /// <param name="look">Which style, and what it needs to build one.</param>
    /// <returns>A renderer for that style.</returns>
    public IPlanetRenderer CreatePlanetRenderer(IViewSurface surface, PlanetLook look);
}
