// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Planets;

internal sealed class WireframePlanet : IObject
{
    private readonly IEliteDraw _draw;
    private readonly PlanetRenderer _planetRenderer;
    private readonly uint _color;

    internal WireframePlanet(IEliteDraw draw)
    {
        _draw = draw;
        _planetRenderer = new(draw);
        _color = draw.Palette["White"];
    }

    private WireframePlanet(WireframePlanet other)
    {
        _draw = other._draw;
        _planetRenderer = other._planetRenderer;
        _color = other._color;
    }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Vector4[] Rotmat { get; set; } = new Vector4[4];

    public ShipProperties Flags { get; set; }

    public ShipType Type { get; set; } = ShipType.Planet;

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public IObject Clone()
    {
        WireframePlanet planet = new(this);
        this.CopyTo(planet);
        return planet;
    }

    public void Draw()
    {
        (Vector2 Position, float Radius)? v = _planetRenderer.GetPlanetPosition(Location);
        if (v != null)
        {
            _draw.Graphics.DrawCircle(v.Value.Position, v.Value.Radius, _color);
        }
    }
}
