// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Suns;

internal sealed class SolidSun : IObject
{
    private readonly IEliteDraw _draw;
    private readonly uint _color;

    internal SolidSun(IEliteDraw draw)
    {
        _draw = draw;
        _color = _draw.Palette["White"];
    }

    private SolidSun(SolidSun other)
    {
        _draw = other._draw;
        _color = other._color;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Vector4[] Rotmat { get; set; } = new Vector4[4];

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Sun;

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

        _draw.Graphics.DrawLine(s, new(ex, s.Y), _color);
    }
}
