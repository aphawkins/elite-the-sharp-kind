// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using Useful.Graphics;

namespace EliteSharp.Planets;

internal sealed class StripedPlanet : IObject
{
    private readonly PlanetRenderer _planetRenderer;

    /// <summary>
    /// Color map used to generate a striped style planet.
    /// </summary>
    private readonly FastColor[] _stripeColors =
    [
        EliteColors.Purple,
        EliteColors.Purple,
        EliteColors.DarkBlue,
        EliteColors.DarkBlue,
        EliteColors.DarkBlue,
        EliteColors.DarkBlue,
        EliteColors.Blue,
        EliteColors.Blue,
        EliteColors.Blue,
        EliteColors.Blue,
        EliteColors.LightBlue,
        EliteColors.LightBlue,
        EliteColors.LighterGrey,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.LightOrange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.Orange,
        EliteColors.LightOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.DarkOrange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.LightOrange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.Orange,
        EliteColors.LighterGrey,
        EliteColors.LightBlue,
        EliteColors.LightBlue,
        EliteColors.Blue,
        EliteColors.Blue,
        EliteColors.Blue,
        EliteColors.Blue,
        EliteColors.DarkBlue,
        EliteColors.DarkBlue,
        EliteColors.DarkBlue,
        EliteColors.DarkBlue,
        EliteColors.Purple,
        EliteColors.Purple,
    ];

    internal StripedPlanet(IEliteDraw draw)
    {
        _planetRenderer = new(draw);
        GenerateLandscape();
    }

    private StripedPlanet(StripedPlanet other) => _planetRenderer = other._planetRenderer;

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Vector4[] Rotmat { get; set; } = new Vector4[4];

    public ShipType Type { get; set; } = ShipType.Planet;

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public IObject Clone()
    {
        StripedPlanet planet = new(this);
        this.CopyTo(planet);
        return planet;
    }

    public void Draw()
    {
        (Vector2 Position, float Radius)? v = _planetRenderer.GetPlanetPosition(Location);
        if (v != null)
        {
            _planetRenderer.Draw(v.Value.Position, v.Value.Radius, Rotmat);
        }
    }

    /// <summary>
    /// Generate a landscape map.
    /// </summary>
    private void GenerateLandscape()
    {
        for (int y = 0; y <= PlanetRenderer.LandYMax; y++)
        {
            FastColor color = _stripeColors[y * (_stripeColors.Length - 1) / PlanetRenderer.LandYMax];
            for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
            {
                _planetRenderer._landscape[x, y] = color;
            }
        }
    }
}
