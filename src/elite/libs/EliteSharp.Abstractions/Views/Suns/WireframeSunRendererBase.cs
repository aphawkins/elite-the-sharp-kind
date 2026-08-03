// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful;

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// The sun as drawn in a wireframe world: a plain disc, with none of the
/// flare the filled styles scatter round their edge. It is filled rather than
/// outlined because an outline alone reads as a planet.
/// </summary>
public abstract class WireframeSunRendererBase : ISunRenderer
{
    private readonly IViewSurface _surface;

    protected WireframeSunRendererBase(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _surface = surface;
    }

    /// <summary>
    /// Gets the colour the disc is filled with.
    /// </summary>
    protected abstract FastColor Colour { get; }

    public void Draw(SunView sun) => _surface.Graphics.DrawCircleFilled(sun.Centre, sun.Radius, Colour);
}
