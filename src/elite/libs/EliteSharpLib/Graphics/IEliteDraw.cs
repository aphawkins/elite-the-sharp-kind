// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib.Graphics;

/// <summary>
/// The game's drawing, which is also what the views are handed - the three
/// members a view may use are <see cref="IViewSurface"/>, and everything below
/// them is the game's own. A view has no business projecting a ship or
/// starting a frame, and cannot reach either from the other side of the
/// plugin seam.
/// </summary>
internal interface IEliteDraw : IViewSurface
{
    /// <summary>
    /// Gets the perspective projection's focal length in pixels: a point at
    /// model-space x projects to <c>Layout.ViewportCentre.X + (x * Focus / z)</c>.
    /// Derived from the tier's screen height so the field of view is the same
    /// at every tier, and independent of <see cref="ViewLayout.Scale"/>.
    /// </summary>
    public float Focus { get; }

    /// <summary>
    /// Gets what the rendition paints each sort of ship. The scanner reads it
    /// for its lollipops and a ship reads it for the beam it fires, so a Viper
    /// is the same colour in both places.
    /// </summary>
    public ShipColours Ships { get; }

    public void DrawObject(IObject obj);

    public void DrawPolygonFilled(Vector2[] points, float[] depths, FastColor faceColor, float z);

    public void RenderEnd();

    public void RenderStart();

    public void SetFullScreenClipRegion();

    public void SetViewClipRegion();
}
