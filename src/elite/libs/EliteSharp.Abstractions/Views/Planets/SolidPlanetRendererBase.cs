// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful;

namespace EliteSharp.Abstractions.Views.Planets;

/// <summary>
/// A planet as a flat disc of one colour - no surface, no orientation. Which
/// colour is the rendition's, and here that is the whole of the difference,
/// so the subclass supplies it and nothing else.
/// </summary>
public abstract class SolidPlanetRendererBase : IPlanetRenderer
{
    private readonly IViewSurface _surface;

    protected SolidPlanetRendererBase(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _surface = surface;
    }

    /// <summary>
    /// Gets the colour the disc is filled with.
    /// </summary>
    protected abstract FastColor Colour { get; }

    public void Draw(PlanetView planet)
        => _surface.Graphics.DrawCircleFilled(planet.Centre, planet.Radius, Colour);
}
