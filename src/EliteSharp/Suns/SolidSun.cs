// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Suns;

internal sealed class SolidSun : IObject
{
    private readonly IDraw _draw;

    internal SolidSun(IDraw draw, in FastColor color)
    {
        _draw = draw;
        Color = color;
    }

    private SolidSun(SolidSun other)
    {
        _draw = other._draw;
        Color = other.Color;
    }

    public ShipProperties Flags { get; set; }

    public Vector3 Location { get; set; } = new(0, 0, 123456);

    public Vector3[] Rotmat { get; set; } = new Vector3[3];

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Sun;

    internal FastColor Color { get; }

    public IObject Clone()
    {
        SolidSun sun = new(this);
        this.CopyTo(sun);
        return sun;
    }

    public void Draw()
    {
        Vector2 centre = new(Location.X, -Location.Y);

        centre *= 256 / Location.Z;
        centre += _draw.Centre / 2;
        centre *= _draw.Graphics.Scale;

        float radius = 6291456 / Location.Length() * _draw.Graphics.Scale;

        if (centre.X + radius < _draw.Left ||
            centre.X - radius > _draw.Right ||
            centre.Y + radius < _draw.Top ||
            centre.Y - radius > _draw.Bottom)
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

    private void RenderSunLine(Vector2 centre, float x, float y, float radius)
    {
        Vector2 s = new()
        {
            Y = centre.Y + y,
        };

        if (s.Y < _draw.Top || s.Y > _draw.Bottom)
        {
            return;
        }

        s.X = centre.X - x;
        float ex = centre.X + x;

        s.X -= radius * RNG.Random(2, 10) / 256f;
        ex += radius * RNG.Random(2, 10) / 256f;

        if (ex < _draw.Left || s.X > _draw.Right)
        {
            return;
        }

        if (s.X < _draw.Left)
        {
            s.X = _draw.Left;
        }

        if (ex > _draw.Right)
        {
            ex = _draw.Right;
        }

        _draw.Graphics.DrawLine(s, new(ex, s.Y), Color);
    }
}
