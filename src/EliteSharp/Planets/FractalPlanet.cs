// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Planets;

internal sealed class FractalPlanet : IObject
{
    private readonly IDraw _draw;

    private readonly PlanetRenderer _planetRenderer;

    internal FractalPlanet(IDraw draw, int seed)
    {
        _draw = draw;
        Seed = seed;
        _planetRenderer = new(draw);
        GenerateLandscape(seed);
    }

    private FractalPlanet(FractalPlanet other)
    {
        _draw = other._draw;
        Seed = other.Seed;
        _planetRenderer = other._planetRenderer;
    }

    public ShipProperties Flags { get; set; }

    public Vector3 Location { get; set; } = new(0, 0, 123456);

    public Vector3[] Rotmat { get; set; } = new Vector3[3];

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; } = ShipType.Planet;

    internal int Seed { get; }

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
    private FastColor CalcMidpointColor(int sx, int sy, int ex, int ey)
        => new(Math.Clamp(
            ((_planetRenderer._landscape[sx, sy].Argb + _planetRenderer._landscape[ex, ey].Argb) / 2) + (uint)RNG.GaussianRandom(-7, 8),
            0,
            255));

    /// <summary>
    /// Generate a fractal landscape. Uses midpoint displacement method.
    /// </summary>
    /// <param name="seed">Initial seed for the generation.</param>
    [SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Randomness here requires seed.")]
    private void GenerateLandscape(int seed)
    {
        const int d = PlanetRenderer.LandXMax / 8;
        Random random = new(seed);

        for (int y = 0; y <= PlanetRenderer.LandYMax; y += d)
        {
            for (int x = 0; x <= PlanetRenderer.LandXMax; x += d)
            {
                _planetRenderer._landscape[x, y] = new((uint)random.Next(255));
            }
        }

        for (int y = 0; y < PlanetRenderer.LandYMax; y += d)
        {
            for (int x = 0; x < PlanetRenderer.LandXMax; x += d)
            {
                MidpointSquare(x, y, d);
            }
        }

        for (int y = 0; y <= PlanetRenderer.LandYMax; y++)
        {
            for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
            {
                float dist = (x * x) + (y * y);
                bool dark = dist > 10000;
                FastColor color = _planetRenderer._landscape[x, y];
                _planetRenderer._landscape[x, y] = color.Argb > 166
                    ? (dark ? EliteColors.Green : EliteColors.LightGreen)
                    : (dark ? EliteColors.Blue : EliteColors.LightBlue);
            }
        }
    }

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

        _planetRenderer._landscape[mx, ty] = CalcMidpointColor(tx, ty, bx, ty);
        _planetRenderer._landscape[mx, by] = CalcMidpointColor(tx, by, bx, by);
        _planetRenderer._landscape[tx, my] = CalcMidpointColor(tx, ty, tx, by);
        _planetRenderer._landscape[bx, my] = CalcMidpointColor(bx, ty, bx, by);
        _planetRenderer._landscape[mx, my] = CalcMidpointColor(tx, my, bx, my);

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
