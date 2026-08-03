// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Useful;

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// A banded sun. The banding maths is shared; which colours the bands are is
/// each tier's own, so <see cref="SunColor"/> is left to the subclass.
/// </summary>
public abstract class GradientSunRendererBase : ISunRenderer
{
    private readonly IViewSurface _surface;
    private readonly IRandomSource _rng;

    protected GradientSunRendererBase(IViewSurface surface, IRandomSource random)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _surface = surface;
        _rng = random;
    }

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

    /// <summary>
    /// The colour for a pixel at the given squared distance from the centre,
    /// against the three squared band radii. <paramref name="dither"/> is the
    /// parity used to mix the outermost band.
    /// </summary>
    protected abstract FastColor SunColor(float distance, float inner, float inner2, float outer, int dither);

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

        float inner = radius * (200 + _rng.Random(8)) / 256;
        inner *= inner;

        float inner2 = radius * (220 + _rng.Random(8)) / 256;
        inner2 *= inner2;

        float outer = radius * (239 + _rng.Random(8)) / 256;
        outer *= outer;

        float dy = y * y;
        float dx = s.X - centre.X;

        for (; s.X <= ex; s.X++, dx++)
        {
            float distance = (dx * dx) + dy;

            _surface.Graphics.DrawPixel(s, SunColor(distance, inner, inner2, outer, (int)s.X ^ (int)y));
        }
    }
}
