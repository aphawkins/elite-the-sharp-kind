// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Useful.Graphics;

namespace EliteSharpLib.Views;

/// <summary>
/// The chrome and text helpers every screen shares, in a tier-specific
/// implementation. Nothing that puts pixels on screen lives in a single
/// class serving both tiers: each tier authors its own spacing, fonts and
/// colours, so these are per-tier too.
/// </summary>
internal interface IBaseView
{
    public IGraphics Graphics { get; }

    public ViewLayout Layout { get; }

    /// <summary>
    /// Draws the screen border framing the view area.
    /// </summary>
    public void DrawBorder();

    /// <summary>
    /// Draws the hyperspace countdown digit in the top-left.
    /// </summary>
    public void DrawHyperspaceCountdown(int countdown);

    /// <summary>
    /// Word-wraps <paramref name="text"/> within the given width.
    /// </summary>
    public void DrawTextPretty(Vector2 position, float width, string text);

    /// <summary>
    /// Draws a screen's title and the rules framing the view area.
    /// </summary>
    public void DrawViewHeader(string title);
}
