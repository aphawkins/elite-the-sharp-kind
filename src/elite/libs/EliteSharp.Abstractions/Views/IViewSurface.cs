// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Assets.Palettes;
using Useful.Graphics;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// Everything a view is given to draw with: somewhere to draw, the metrics to
/// lay out against, and the tier's colours. This is the whole of it - a view
/// rendition sees no more of the game than these three members.
/// <para>
/// The game's own drawing interface implements this rather than being it. A
/// view has no business projecting a ship or starting a frame, so those stay
/// on the game's side of the seam and never appear here.
/// </para>
/// </summary>
public interface IViewSurface
{
    /// <summary>
    /// Gets the surface to draw on.
    /// </summary>
    public IGraphics Graphics { get; }

    /// <summary>
    /// Gets the tier's screen metrics for laying out against.
    /// </summary>
    public ViewLayout Layout { get; }

    /// <summary>
    /// Gets the tier's palette. The two tiers' colour names need not overlap,
    /// so a rendition looks up only names its own tier's palette defines.
    /// </summary>
    public IPaletteCollection Palette { get; }
}
