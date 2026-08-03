// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Useful;

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// A filled sun with a flaring rim: each scanline is stretched a random
/// amount past the circle, which is what gives the edge its shimmer.
/// </summary>
public abstract class SolidSunRendererBase : ISunRenderer
{
    private readonly IViewSurface _surface;
    private readonly IRandomSource _rng;

    protected SolidSunRendererBase(IViewSurface surface, IRandomSource random)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _surface = surface;
        _rng = random;
    }

    /// <summary>
    /// Gets the colour the disc is filled with.
    /// </summary>
    protected abstract FastColor Colour { get; }

    public void Draw(SunView sun)
    {
        Vector2 centre = sun.Centre;
        float radius = sun.Radius;

        float s = -radius;
        float x = radius;
        float y = 0;

        while (y <= x)
        {
            // Top of top half
            RenderSunLine(centre, y, -MathF.Floor(x), radius);

            // Top of top half
            RenderSunLine(centre, x, -y, radius);

            // Top of bottom half
            RenderSunLine(centre, x, y, radius);

            // Bottom of bottom half
            RenderSunLine(centre, y, MathF.Floor(x), radius);

            s += y + y + 1;
            y++;
            if (s >= 0)
            {
                s -= x + x + 2;
                x--;
            }
        }
    }

    private void RenderSunLine(Vector2 centre, float x, float y, float radius)
    {
        Vector2 s = new()
        {
            Y = centre.Y + y,
        };

        if (s.Y < _surface.Layout.ViewportTop || s.Y > _surface.Layout.ViewportBottom)
        {
            return;
        }

        s.X = centre.X - x;
        float ex = centre.X + x;

        s.X -= radius * _rng.Random(2, 10) / 256f;
        ex += radius * _rng.Random(2, 10) / 256f;

        if (ex < _surface.Layout.ViewportLeft || s.X > _surface.Layout.ViewportRight)
        {
            return;
        }

        if (s.X < _surface.Layout.ViewportLeft)
        {
            s.X = _surface.Layout.ViewportLeft;
        }

        if (ex > _surface.Layout.ViewportRight)
        {
            ex = _surface.Layout.ViewportRight;
        }

        _surface.Graphics.DrawLine(s, new(ex, s.Y), Colour);
    }
}
