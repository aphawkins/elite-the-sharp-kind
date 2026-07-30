// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib.Planets;

/// <summary>
/// A banded, Jupiter-like planet. The banding is shared; the stripe colours
/// are each tier's own, so the subclass supplies the map.
/// </summary>
internal abstract class StripedPlanetBase : IObject
{
    private readonly PlanetRenderer _planetRenderer;

    protected StripedPlanetBase(IEliteDraw draw) => _planetRenderer = new(draw);

    protected StripedPlanetBase(StripedPlanetBase other)
    {
        ArgumentNullException.ThrowIfNull(other);

        _planetRenderer = other._planetRenderer;
    }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public ShipType Type { get; set; } = ShipType.Planet;

    public float RotX { get; set; }

    public float RotZ { get; set; }

    /// <summary>
    /// Gets the colour map the bands are taken from, pole to pole.
    /// </summary>
    protected abstract FastColor[] StripeColors { get; }

    public abstract IObject Clone();

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
    protected void GenerateLandscape()
    {
        FastColor[] stripes = StripeColors;

        for (int y = 0; y <= PlanetRenderer.LandYMax; y++)
        {
            FastColor color = stripes[y * (stripes.Length - 1) / PlanetRenderer.LandYMax];
            for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
            {
                _planetRenderer.Landscape[x, y] = color;
            }
        }
    }
}
