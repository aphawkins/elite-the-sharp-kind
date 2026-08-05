// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful;

namespace EliteSharp.Abstractions.Views.Stars;

/// <summary>
/// A star is a dot, or a line when it is streaking past. Nearer ones are
/// drawn a pixel wider, which is the whole of what the original did to
/// suggest depth.
/// </summary>
public abstract class StarfieldRendererBase : IStarfieldRenderer
{
    private readonly IViewSurface _surface;

    protected StarfieldRendererBase(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _surface = surface;
    }

    public abstract int NormalSpaceStarCount { get; }

    public abstract int WitchspaceStarCount { get; }

    /// <summary>
    /// Gets the colour stars are drawn in.
    /// </summary>
    protected abstract FastColor Colour { get; }

    /// <summary>
    /// Gets the colour a near star's extra pixels (beyond its single-pixel
    /// core) are drawn in. Defaults to <see cref="Colour"/>, matching the
    /// original's flat look; a rendition can dim it instead to soften a
    /// growing star's edge rather than block it out in solid colour.
    /// </summary>
    protected virtual FastColor HaloColour => Colour;

    /// <summary>
    /// Gets the distance a star has to be inside to be drawn a pixel wider.
    /// </summary>
    protected abstract float WideDistance { get; }

    /// <summary>
    /// Gets the distance a star has to be inside to fill a two-by-two block,
    /// which is as near as the original let one look.
    /// </summary>
    protected abstract float BlockDistance { get; }

    public void Draw(IReadOnlyList<StarMark> stars)
    {
        ArgumentNullException.ThrowIfNull(stars);

        foreach (StarMark star in stars)
        {
            if (star.IsStreaking)
            {
                _surface.Graphics.DrawLine(star.Position, star.StreakTo, Colour);
                continue;
            }

            _surface.Graphics.DrawPixel(star.Position, Colour);

            if (star.Distance < WideDistance)
            {
                _surface.Graphics.DrawPixel(new(star.Position.X + 1, star.Position.Y), HaloColour);
            }

            if (star.Distance < BlockDistance)
            {
                _surface.Graphics.DrawPixel(new(star.Position.X, star.Position.Y + 1), HaloColour);
                _surface.Graphics.DrawPixel(new(star.Position.X + 1, star.Position.Y + 1), HaloColour);
            }
        }
    }
}
