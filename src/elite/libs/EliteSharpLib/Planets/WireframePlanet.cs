// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Planets;

internal sealed class WireframePlanet : IObject
{
    // The original draws no surface detail below a radius of 6 (PL9).
    private const float MinDetailRadius = 6;

    // Segments per half ellipse; the original steps a 64-segment circle by 4.
    private const int HalfEllipseSegments = 8;

    // The crater's centre is offset from the planet's by 222/256 of the radius
    // along roofv (PLS3).
    private const float CraterOffset = 222f / 256;

    private readonly IEliteDraw _draw;
    private readonly PlanetRenderer _planetRenderer;
    private readonly uint _color;
    private readonly bool _hasCrater;

    internal WireframePlanet(IEliteDraw draw, bool hasCrater)
    {
        _draw = draw;
        _planetRenderer = new(draw);
        _color = draw.Palette["White"];
        _hasCrater = hasCrater;
    }

    private WireframePlanet(WireframePlanet other)
    {
        _draw = other._draw;
        _planetRenderer = other._planetRenderer;
        _color = other._color;
        _hasCrater = other._hasCrater;
    }

    public Vector4 Location { get; set; } = new(0, 0, 123456, 0);

    public Matrix4x4 Rotmat { get; set; }

    public ShipProperties Flags { get; set; }

    public ShipType Type { get; set; } = ShipType.Planet;

    // Pitch and roll pegged at 127, as the original (SOS1), so the planet keeps
    // turning without damping and its surface detail sweeps round. Only this
    // style spins: the others map their surface from Rotmat's rows and expect
    // it to stay put.
    public float RotX { get; set; } = 127;

    public float RotZ { get; set; } = 127;

    public IObject Clone()
    {
        WireframePlanet planet = new(this);
        this.CopyTo(planet);
        return planet;
    }

    public void Draw()
    {
        (Vector2 Position, float Radius)? v = _planetRenderer.GetPlanetPosition(Location);
        if (v == null)
        {
            return;
        }

        (Vector2 centre, float radius) = v.Value;
        _draw.Graphics.DrawCircle(centre, radius, _color);

        if (radius < MinDetailRadius * _draw.Scale)
        {
            return;
        }

        // The orientation vector rows, as the original's sidev/roofv/nosev.
        Vector3 sidev = new(Rotmat.M11, Rotmat.M12, Rotmat.M13);
        Vector3 roofv = new(Rotmat.M21, Rotmat.M22, Rotmat.M23);
        Vector3 nosev = new(Rotmat.M31, Rotmat.M32, Rotmat.M33);

        if (_hasCrater)
        {
            DrawCrater(centre, radius, sidev, roofv, nosev);
        }
        else
        {
            DrawEquatorAndMeridian(centre, radius, sidev, roofv, nosev);
        }
    }

    /// <summary>
    /// The starting angle of a meridian, as the original's PLS4:
    /// arctan(-nosev_z / other_z), reduced to half a turn, plus half a turn
    /// again when nosev_z is positive.
    /// </summary>
    private static float MeridianStartAngle(float nosevZ, float otherZ)
    {
        float angle = MathF.Atan2(-nosevZ, otherZ);

        if (angle < 0)
        {
            angle += MathF.PI;
        }

        return nosevZ >= 0 ? angle + MathF.PI : angle;
    }

    /// <summary>
    /// A point on the ellipse defined by the centre and the two conjugate
    /// radius vectors u and v (PLS22). The y components are negated because
    /// screen y runs the opposite way to y in space.
    /// </summary>
    private static Vector2 EllipsePoint(Vector2 centre, Vector2 u, Vector2 v, float angle)
    {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        return new(
            centre.X + (u.X * cos) + (v.X * sin),
            centre.Y - (u.Y * cos) - (v.Y * sin));
    }

    /// <summary>
    /// Draw the planet's equator and meridian: two half ellipses sharing
    /// nosev, one against roofv and one against sidev (PL9 part 2).
    /// </summary>
    private void DrawEquatorAndMeridian(Vector2 centre, float radius, Vector3 sidev, Vector3 roofv, Vector3 nosev)
    {
        Vector2 u = new(nosev.X * radius, nosev.Y * radius);

        DrawEllipseArc(
            centre,
            u,
            new(roofv.X * radius, roofv.Y * radius),
            MeridianStartAngle(nosev.Z, roofv.Z),
            MathF.PI,
            HalfEllipseSegments);

        DrawEllipseArc(
            centre,
            u,
            new(sidev.X * radius, sidev.Y * radius),
            MeridianStartAngle(nosev.Z, sidev.Z),
            MathF.PI,
            HalfEllipseSegments);
    }

    /// <summary>
    /// Draw the planet's crater: a full ellipse of half the planet's radius,
    /// offset along roofv (PL9 part 3).
    /// </summary>
    private void DrawCrater(Vector2 centre, float radius, Vector3 sidev, Vector3 roofv, Vector3 nosev)
    {
        if (roofv.Z < 0)
        {
            // The crater is on the far side of the planet.
            return;
        }

        Vector2 craterCentre = new(
            centre.X + (roofv.X * radius * CraterOffset),
            centre.Y - (roofv.Y * radius * CraterOffset));

        DrawEllipseArc(
            craterCentre,
            new(nosev.X * radius / 2, nosev.Y * radius / 2),
            new(sidev.X * radius / 2, sidev.Y * radius / 2),
            0,
            MathF.Tau,
            HalfEllipseSegments * 2);
    }

    private void DrawEllipseArc(Vector2 centre, Vector2 u, Vector2 v, float startAngle, float sweep, int segments)
    {
        Vector2 start = EllipsePoint(centre, u, v, startAngle);

        for (int i = 1; i <= segments; i++)
        {
            Vector2 end = EllipsePoint(centre, u, v, startAngle + (sweep * i / segments));
            _draw.Graphics.DrawLine(start, end, _color);
            start = end;
        }
    }
}
