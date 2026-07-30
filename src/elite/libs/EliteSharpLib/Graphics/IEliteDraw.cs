// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using Useful;
using Useful.Assets;
using Useful.Assets.Palettes;
using Useful.Graphics;

namespace EliteSharpLib.Graphics;

internal interface IEliteDraw
{
    /// <summary>
    /// Gets the tier's screen metrics for laying out against.
    /// </summary>
    public ViewLayout Layout { get; }

    /// <summary>
    /// Gets the machine class being reproduced, which the object factories
    /// pick their per-tier renderers by.
    /// </summary>
    public SystemTier Tier { get; }

    /// <summary>
    /// Gets the perspective projection's focal length in pixels: a point at
    /// model-space x projects to <c>Layout.Centre.X + (x * Focus / z)</c>.
    /// Derived from the tier's screen height so the field of view is the same
    /// at every tier, and independent of <see cref="ViewLayout.Scale"/>.
    /// </summary>
    public float Focus { get; }

    public IGraphics Graphics { get; }

    public IPaletteCollection Palette { get; }

    public void DrawObject(IObject obj);

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor, float z);

    public void RenderEnd();

    public void RenderStart();

    public void SetFullScreenClipRegion();

    public void SetViewClipRegion();
}
