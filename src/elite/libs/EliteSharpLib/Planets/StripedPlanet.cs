// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Planets;

internal sealed class StripedPlanet : IObject
{
    private readonly PlanetRenderer _planetRenderer;

    // Color map used to generate a striped style planet.
    private readonly uint[] _stripeColors;

    internal StripedPlanet(IEliteDraw draw)
    {
        _planetRenderer = new(draw);
        uint colorPurple = draw.Palette["Purple"];
        uint colorDarkBlue = draw.Palette["DarkBlue"];
        uint colorBlue = draw.Palette["Blue"];
        uint colorLightBlue = draw.Palette["LightBlue"];
        uint colorLighterGrey = draw.Palette["LighterGrey"];
        uint colorOrange = draw.Palette["Orange"];
        uint colorLightOrange = draw.Palette["LightOrange"];
        uint colorDarkOrange = draw.Palette["DarkOrange"];
        _stripeColors =
        [
            colorPurple,
            colorPurple,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorLightBlue,
            colorLightBlue,
            colorLighterGrey,
            colorOrange,
            colorOrange,
            colorOrange,
            colorOrange,
            colorLightOrange,
            colorOrange,
            colorOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorOrange,
            colorLightOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorDarkOrange,
            colorOrange,
            colorOrange,
            colorLightOrange,
            colorOrange,
            colorOrange,
            colorOrange,
            colorOrange,
            colorLighterGrey,
            colorLightBlue,
            colorLightBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorDarkBlue,
            colorPurple,
            colorPurple,
        ];

        GenerateLandscape();
    }

    private StripedPlanet(StripedPlanet other)
    {
        _planetRenderer = other._planetRenderer;
        _stripeColors = other._stripeColors;
    }

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
            uint color = _stripeColors[y * (_stripeColors.Length - 1) / PlanetRenderer.LandYMax];
            for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
            {
                _planetRenderer._landscape[x, y] = color;
            }
        }
    }
}
