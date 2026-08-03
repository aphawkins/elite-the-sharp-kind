// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views.Planets;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Planets;

/// <summary>
/// A planet in the universe. It has a position, it turns, it is cloned into
/// witchspace and back - all of which is the game's - and what it looks like
/// is the rendition's, through <see cref="IPlanetRenderer"/>.
/// <para>
/// There is one of these whatever the style. The styles used to be four
/// classes here, each repeating the same position and cloning around a
/// different few lines of drawing.
/// </para>
/// </summary>
internal sealed class Planet : IObject
{
    private readonly IEliteDraw _draw;
    private readonly IPlanetRenderer _renderer;

    internal Planet(IEliteDraw draw, IPlanetRenderer renderer, bool spins)
    {
        _draw = draw;
        _renderer = renderer;

        if (spins)
        {
            // Pitch and roll pegged at 127, as the original (SOS1), so the
            // planet keeps turning without damping and its surface detail
            // sweeps round. Only the outlined style does this: the surfaced
            // ones map their detail from Rotmat's rows and expect it to stay
            // put.
            RotX = 127;
            RotZ = 127;
        }
    }

    private Planet(Planet other)
    {
        _draw = other._draw;
        _renderer = other._renderer;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Planet;

    public IObject Clone()
    {
        Planet planet = new(this);
        this.CopyTo(planet);
        return planet;
    }

    public void Draw()
    {
        Vector2 centre = new(Location.X, -Location.Y);
        centre *= _draw.Focus / Location.Z;
        centre += _draw.Layout.ViewportCentre;

        // Planets are BIG! The radius is in the original's 256-wide space, so
        // it follows the projection's focal length rather than the scale.
        float unitScale = _draw.Focus / 256;
        float radius = 6291456 / Location.Length() * unitScale;

        if ((centre.X + radius < _draw.Layout.ViewportLeft) ||
            (centre.X - radius > _draw.Layout.ViewportRight) ||
            (centre.Y + radius < _draw.Layout.ViewportTop) ||
            (centre.Y - radius > _draw.Layout.ViewportBottom))
        {
            return;
        }

        _renderer.Draw(new(centre, radius, Rotmat, unitScale));
    }
}
