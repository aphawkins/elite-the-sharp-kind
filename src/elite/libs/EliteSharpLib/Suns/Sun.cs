// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views.Suns;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Suns;

/// <summary>
/// A sun in the universe. As with a planet, where it is and how it is cloned
/// are the game's, and what it looks like is the rendition's through
/// <see cref="ISunRenderer"/>.
/// </summary>
internal sealed class Sun : IObject
{
    private readonly IEliteDraw _draw;
    private readonly ISunRenderer _renderer;

    internal Sun(IEliteDraw draw, ISunRenderer renderer)
    {
        _draw = draw;
        _renderer = renderer;
    }

    private Sun(Sun other)
    {
        _draw = other._draw;
        _renderer = other._renderer;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Sun;

    public IObject Clone()
    {
        Sun sun = new(this);
        this.CopyTo(sun);
        return sun;
    }

    public void Draw()
    {
        if (WorldProjection.TryProject(_draw, Location, out Vector2 centre, out float radius, out _))
        {
            _renderer.Draw(new(centre, radius));
        }
    }
}
