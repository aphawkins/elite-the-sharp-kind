// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;
using Useful.Assets;

namespace EliteSharpLib.Planets;

internal sealed class FractalPlanet : IObject
{
    private readonly IEliteDraw _draw;
    private readonly PlanetRenderer _planetRenderer;
    private readonly FastColor _colorDarkSea;
    private readonly FastColor _colorDarkLand;
    private readonly FastColor _colorSea;
    private readonly FastColor _colorLand;
    private readonly IRandomSource _landscapeRandom;

    // The midpoint-displacement pass works in heights, not colours; ColorLandscape
    // then maps each height onto the renderer's landscape.
    private readonly uint[,] _heights =
        new uint[PlanetRenderer.LandXMax + 1, PlanetRenderer.LandYMax + 1];

    internal FractalPlanet(IEliteDraw draw, int seed)
    {
        _draw = draw;
        Seed = seed;

        // Reference: fesh0r/newkind's generate_fractal_landscape(rnd_seed) reseeds a
        // single stream for the whole landscape (corner grid + jitter), so the same
        // system always renders a byte-identical planet.
        Random random = new(seed);
        _landscapeRandom = new RandomSource(random);
        _planetRenderer = new(draw);

        // Each tier names its own colours, so the sea and land shades are
        // looked up by role rather than by a name shared across palettes.
        bool eightBit = draw.Tier == SystemTier.EightBit;
        _colorDarkSea = draw.Palette[eightBit ? "Blue" : "Navy"];
        _colorDarkLand = draw.Palette["Green"];
        _colorSea = draw.Palette[eightBit ? "LightBlue" : "Teal"];
        _colorLand = draw.Palette[eightBit ? "LightGreen" : "YellowGreen"];

        GenerateLandscape();
    }

    private FractalPlanet(FractalPlanet other)
    {
        _draw = other._draw;
        Seed = other.Seed;
        _landscapeRandom = other._landscapeRandom;
        _planetRenderer = other._planetRenderer;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Planet;

    internal int Seed { get; }

    internal FastColor[,] Landscape => _planetRenderer.Landscape;

    public IObject Clone()
    {
        FractalPlanet planet = new(this);
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
    /// Calculate the midpoint between two given points.
    /// </summary>
    private uint CalcMidpointColor(int sx, int sy, int ex, int ey)
        => Math.Clamp(
            ((_heights[sx, sy] + _heights[ex, ey]) / 2) + (uint)_landscapeRandom.GaussianRandom(-7, 8),
            0,
            255);

    /// <summary>
    /// Generate a fractal landscape. Uses midpoint displacement method.
    /// </summary>
    private void GenerateLandscape()
    {
        const int d = PlanetRenderer.LandXMax / 8;

        for (int y = 0; y <= PlanetRenderer.LandYMax; y += d)
        {
            for (int x = 0; x <= PlanetRenderer.LandXMax; x += d)
            {
                _heights[x, y] = (uint)_landscapeRandom.Random(255);
            }
        }

        for (int y = 0; y < PlanetRenderer.LandYMax; y += d)
        {
            for (int x = 0; x < PlanetRenderer.LandXMax; x += d)
            {
                MidpointSquare(x, y, d);
            }
        }

        ColorLandscape();
    }

    /// <summary>
    /// Turn the generated heightmap into land and sea, shaded by distance from
    /// the light source at the top left.
    /// </summary>
    private void ColorLandscape()
    {
        for (int y = 0; y <= PlanetRenderer.LandYMax; y++)
        {
            for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
            {
                float dist = (x * x) + (y * y);
                bool dark = dist > 10000;
                _planetRenderer.Landscape[x, y] = LandscapeColor(_heights[x, y], dark);
            }
        }
    }

    private FastColor LandscapeColor(uint height, bool dark)
        => height > 166
            ? (dark ? _colorDarkLand : _colorLand)
            : (dark ? _colorDarkSea : _colorSea);

    /// <summary>
    /// Calculate a square on the midpoint map.
    /// </summary>
    private void MidpointSquare(int tx, int ty, int w)
    {
        int d = w / 2;
        int mx = tx + d;
        int my = ty + d;
        int bx = tx + w;
        int by = ty + w;

        _heights[mx, ty] = CalcMidpointColor(tx, ty, bx, ty);
        _heights[mx, by] = CalcMidpointColor(tx, by, bx, by);
        _heights[tx, my] = CalcMidpointColor(tx, ty, tx, by);
        _heights[bx, my] = CalcMidpointColor(bx, ty, bx, by);
        _heights[mx, my] = CalcMidpointColor(tx, my, bx, my);

        if (d == 1)
        {
            return;
        }

        MidpointSquare(tx, ty, d);
        MidpointSquare(mx, ty, d);
        MidpointSquare(tx, my, d);
        MidpointSquare(mx, my, d);
    }
}
