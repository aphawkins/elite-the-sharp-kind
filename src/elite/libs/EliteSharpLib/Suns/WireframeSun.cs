// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib.Suns;

// The sun as drawn in a wireframe world: a plain white disc, with none of the
// flare the filled styles scatter round their edge. It is filled rather than
// outlined because an outline alone reads as a planet.
internal sealed class WireframeSun : IObject
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _color;

    internal WireframeSun(IEliteDraw draw)
    {
        _draw = draw;
        _color = draw.Palette["White"];
    }

    private WireframeSun(WireframeSun other)
    {
        _draw = other._draw;
        _color = other._color;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Sun;

    public IObject Clone()
    {
        WireframeSun sun = new(this);
        this.CopyTo(sun);
        return sun;
    }

    public void Draw()
    {
        Vector2 centre = new(Location.X, -Location.Y);

        centre *= _draw.Focus / Location.Z;
        centre += _draw.Layout.Centre;

        // The same size as the filled suns: a radius in the original's
        // 256-wide space, scaled to the tier's screen.
        float radius = 6291456 / Location.Length() * (_draw.Focus / 256);

        if (centre.X + radius < _draw.Layout.Left ||
            centre.X - radius > _draw.Layout.Right ||
            centre.Y + radius < _draw.Layout.Top ||
            centre.Y - radius > _draw.Layout.Bottom)
        {
            return;
        }

        _draw.Graphics.DrawCircleFilled(centre, radius, _color);
    }
}
