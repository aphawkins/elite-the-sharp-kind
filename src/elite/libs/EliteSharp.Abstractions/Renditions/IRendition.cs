// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using Useful.Assets;

namespace EliteSharp.Abstractions.Renditions;

/// <summary>
/// Everything one asset tier draws, written in its own assembly and found at
/// startup - the same door the missions come through. A tier's whole
/// presentation is then an assembly rather than a branch in the game's
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
    /// Gets the tier this rendition draws. The game loads every rendition it finds and
    /// uses the one whose tier the commander configured.
    /// </summary>
    public SystemTier Tier { get; }

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
}
