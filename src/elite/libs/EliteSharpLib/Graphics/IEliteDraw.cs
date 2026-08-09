// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Ships;
using SharpKind;
using SharpKind.Graphics;

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
    /// Gets the perspective projection itself - <see cref="Focus"/> about the
    /// viewport centre. Everything that puts a camera-space point on screen
    /// goes through this, so there is one projection and not five.
    /// </summary>
    public PerspectiveProjector Projector => new(Focus, Layout.ViewportCentre);

    /// <summary>
    /// Gets what the rendition paints each sort of ship. The scanner reads it
    /// for its lollipops and a ship reads it for the beam it fires, so a Viper
    /// is the same colour in both places.
    /// </summary>
    public ShipColours Ships { get; }

    /// <summary>
    /// One face's colour as this rendition's lighting leaves it. Returns the
    /// colour untouched when the rendition does not shade, when the commander
    /// turned lighting off, or when the world is drawn as outlines - so a
    /// caller shades by calling this and never by asking whether to.
    /// </summary>
    /// <param name="faceColour">The model's own colour for the face.</param>
    /// <param name="cameraNormal">The face's normal, rotated into camera space.</param>
    /// <param name="fullyLit">
    /// The brightest channel the model paints anywhere, which is where a face
    /// turned into the light tops out. Keeps a lit model no brighter than the
    /// hand-painted one it replaces.
    /// </param>
    /// <returns>The colour to fill the face with.</returns>
    public FastColor ShadeFace(FastColor faceColour, Vector3 cameraNormal, byte fullyLit);

    public void DrawObject(IObject obj);

    public void DrawPolygonFilled(Vector2[] points, float[] depths, FastColor faceColor, float z);

    public void RenderEnd();

    public void RenderStart();

    public void SetFullScreenClipRegion();

    public void SetViewClipRegion();
}
