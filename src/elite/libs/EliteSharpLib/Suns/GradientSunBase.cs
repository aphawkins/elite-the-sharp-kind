// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib.Suns;

/// <summary>
/// A banded sun. The banding maths is shared; which colours the bands are is
/// each tier's own, so <see cref="SunColor"/> is left to the subclass.
/// </summary>
internal abstract class GradientSunBase : IObject
{
    private readonly IEliteDraw _draw;
    private readonly RNG _rng;

    protected GradientSunBase(IEliteDraw draw, RNG rng)
    {
        _draw = draw;
        _rng = rng;
    }

    protected GradientSunBase(GradientSunBase other)
    {
        ArgumentNullException.ThrowIfNull(other);

        _draw = other._draw;
        _rng = other._rng;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Sun;

    public abstract IObject Clone();

    public void Draw()
    {
        Vector2 centre = new(Location.X, -Location.Y);

        centre *= _draw.Focus / Location.Z;
        centre += _draw.Layout.ViewportCentre;

        float radius = 6291456 / Location.Length() * (_draw.Focus / 256);

        if (centre.X + radius < _draw.Layout.ViewportLeft ||
            centre.X - radius > _draw.Layout.ViewportRight ||
            centre.Y + radius < _draw.Layout.ViewportTop ||
            centre.Y - radius > _draw.Layout.ViewportBottom)
        {
            return;
        }

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

        if (s.Y < _draw.Layout.ViewportTop || s.Y > _draw.Layout.ViewportBottom)
        {
            return;
        }

        s.X = centre.X - x;
        float ex = centre.X + x;

        s.X -= radius * _rng.Random(2, 10) / 256f;
        ex += radius * _rng.Random(2, 10) / 256f;

        if (ex < _draw.Layout.ViewportLeft || s.X > _draw.Layout.ViewportRight)
        {
            return;
        }

        if (s.X < _draw.Layout.ViewportLeft)
        {
            s.X = _draw.Layout.ViewportLeft;
        }

        if (ex > _draw.Layout.ViewportRight)
        {
            ex = _draw.Layout.ViewportRight;
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

            _draw.Graphics.DrawPixel(s, SunColor(distance, inner, inner2, outer, (int)s.X ^ (int)y));
        }
    }
}
